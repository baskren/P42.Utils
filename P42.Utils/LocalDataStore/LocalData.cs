using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Web;
using P42.Serilog.QuickLog;

namespace P42.Utils;



/// <summary>
/// base for storage/recall of items in local data store for common caching scenarios
/// </summary>
public abstract class LocalData
{

    #region Fields

    internal static readonly SemaphoreSlim Semaphore = new (1, 1);
    

    protected static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> ItemLoadingTasks = new();
    [JetBrains.Annotations.PublicAPI]
    protected static readonly HttpClient HttpClient = new();
    // ReSharper restore StaticMemberInGenericType

    [JetBrains.Annotations.PublicAPI]
    protected static DateTime SessionDateTime = DateTime.Now;

    
    [JetBrains.Annotations.PublicAPI]
    internal static string PlatformFolder => Platform.ApplicationLocalFolderPath;
    [JetBrains.Annotations.PublicAPI]
    internal static readonly string StorePath = Path.Combine(PlatformFolder, "P42.Utils.LocalData");
    #endregion

    
    #region Properties
    public static bool IsLocalDataEmpty => !Directory.Exists(StorePath) || Directory.GetFileSystemEntries(StorePath).Length == 0;

    #endregion


    #region Construction / Initialization

    static LocalData()
    {
        if (!DirectoryExtensions.TryGetOrCreateDirectory(PlatformFolder, out var platformFolder))
            throw new Exception($"Cannot create PlatformFolder [{PlatformFolder}] for P42.Utils.LocalData");

        if (platformFolder.FullName is null)
            throw new Exception($"Cannot get PlatformFolder.FullName [{PlatformFolder}] for P42.Utils.LocalData");

        var asm = AssemblyExtensions.GetApplicationAssembly();
        var version = asm.GetName().Version ?? throw new Exception("Cannot get Version for P42.Utils.LocalData");
        var versionFilePath = Path.Combine(platformFolder.FullName, "P42.Utils.LocalData.Version.txt");

        if (!DirectoryExtensions.TryGetOrCreateDirectory(StorePath, out var storeFolder))
            throw new Exception($"Cannot create StoreFolder [{StorePath}] forP42.Utils.LocalData");
        if (storeFolder is null)
            throw new Exception($"Cannot get StoreFolder [{StorePath}] for P42.Utils.LocalData");

        if (File.Exists(versionFilePath))
        {
            var oldVersion = File.ReadAllText(versionFilePath);
            if (!string.IsNullOrWhiteSpace(oldVersion) && oldVersion == version.ToString())
                return;
            
            //TODO: Right now, the local data store is cleared when the app is updated.  This may not be the best behavior for downloaded items.
            storeFolder.Delete(true);
            storeFolder.Create();
        }
        
        DirectoryExtensions.GetOrCreateParentDirectory(versionFilePath);
        File.WriteAllText(versionFilePath, version.ToString());
    }


    #endregion


    #region List

    /// <summary>
    /// Lists all items in local data store.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static List<string> List(Item key)
    {
        if (File.Exists(key.FullPath))
            return [key.FullPath];

        return Directory.Exists(key.FullPath) 
            ? Directory.EnumerateFiles(key.FullPath, "*", SearchOption.AllDirectories).ToList() 
            : [];
    }

    #endregion


    #region Clear

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static bool Clear(Item? key = null)
        => Clear(DateTime.Now, key);

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="timeSpan">only items equal to or older than</param>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
    public static bool Clear(TimeSpan timeSpan, Item? key = null)
        => Clear(DateTime.Now - timeSpan, key);

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="dateTime">only items equal to or older than</param>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
    [JetBrains.Annotations.PublicAPI]
    public static bool Clear(DateTime dateTime, Item? key = null)
    {
        key ??= TagItem.InternalFor(string.Empty, string.Empty);

        if (string.IsNullOrWhiteSpace(key.FullPath))
            return false;
        
        if (!key.FullPath.StartsWith(StorePath))
            throw new Exception($"Key [{key.FullPath}] does not start with P42.Utils.LocalData Platform.LocalData_StorePath");

        try
        {
            Semaphore.Wait();
            if (File.Exists(key.FullPath))
            {
                if (File.GetLastWriteTime(key.FullPath) > dateTime)
                    return false;

                File.Delete(key.FullPath);
                return true;
            }
            else if (Directory.Exists(key.FullPath))
            {
                if (Directory.GetLastWriteTime(key.FullPath) > dateTime)
                    return false;

                Directory.Delete(key.FullPath, true);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
        finally
        {
            Semaphore.Release();
        }


    }

    #endregion
    

    #region ItemKey

    /// <summary>
    /// Base for LocalData stored Item
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="folderPath"></param>
    /// <param name="assembly"></param>
    public abstract class Item(string fullPath, string? folderPath, Assembly? assembly) : IEquatable<Item>
    {
        /// <summary>
        /// Used to compartmentalize items by assembly (default: current application assembly)
        /// </summary>
        protected Assembly Assembly
        {
            get => field ??= AssemblyExtensions.GetApplicationAssembly();
        } = assembly;

        /// <summary>
        /// FolderPath (used to further compartmentalize items (default: null)
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public string FolderPath { get; } = folderPath?.Trim('/').Trim('\\') ?? string.Empty;

        /// <summary>
        /// File system path to item
        /// </summary>
        public string FullPath { get; } = fullPath;

        /// <summary>
        /// File Uri from FullPath
        /// </summary>
        public Uri FileUri => new (FullPath);

        /// <summary>
        /// "ms-appdata://" path to item
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public string AppDataUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullPath) || !FullPath.StartsWith(PlatformFolder))
                    return string.Empty;

                var localPathFragment = FullPath[PlatformFolder.Length..].Replace('\\', '/').Trim('/');
                return $"ms-appdata:///local/{localPathFragment}";
            }
        }

        /// <summary>
        /// "ms-appdata://" Uri to item
        /// </summary>
        public Uri AppDataUri => new (AppDataUrl);
        

        /// <summary>
        /// Normalize and validate key for consistency
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected static string CleanKey(string key)
        {
            key = key.Trim();
            return !key.IsLegalFileName() 
                ? throw new Exception($"Invalid file name characters found in [{key}] for P42.Utils.LocalData") 
                : key;
        }

        /// <summary> 
        /// Gets path to folder used for local data store.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected static string FullPathForFolderAndAssembly(string? folder, Assembly? assembly)
        {
            folder = folder?.Trim('/').Trim('\\') ?? string.Empty;
            return Path.Combine(StorePath, assembly?.Name() ?? string.Empty, folder);
        }

        /// <summary>
        /// Human-readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"FolderPath: [{FolderPath}]; Assembly: [{Assembly.Name()}];";

        /// <summary>
        /// Is the item stored locally?
        /// </summary>
        public bool Exists => File.Exists(FullPath) || Directory.Exists(FullPath);

        /// <summary>
        /// Is the item stored as a directory?
        /// </summary>
        public bool IsDirectory => Directory.Exists(FullPath);

        /// <summary>
        /// Is the item stored as a file?
        /// </summary>
        public bool IsFile => File.Exists(FullPath);


        /// <summary>
        /// Clear item
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Clear()
        {
            if (!Exists) return;
            if (IsDirectory)
                Directory.Delete(FullPath, true);
            else if (IsFile)
                File.Delete(FullPath);
            else
                throw new Exception($"Unknown file type for ItemKey [{this}]");
        }

        /// <summary>
        /// Try clear item
        /// </summary>
        /// <returns></returns>
        public bool TryClear()
        {
            if (!Exists) return false;

            try
            {
                Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Assures Parent Directory has been created
        /// </summary>
        /// <returns>Parent Directory</returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public DirectoryInfo AssureExistsParentDirectory()
            => DirectoryExtensions.GetOrCreateParentDirectory(FullPath);

        /// <summary>
        /// Assures Parent Directory has been created
        /// </summary>
        /// <param name="parent">Parent Directory</param>
        /// <returns></returns>
        public bool TryAssureParentDirectory([MaybeNullWhen(false)] out DirectoryInfo parent)
        {
            parent = null;
            if (!DirectoryExtensions.TryGetOrCreateParentDirectory(FullPath, out DirectoryInfo result))
                return false;

            parent = result;
            return true;
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(Item? other)
        {
            if (other is null)
                return false;

            return FullPath == other.FullPath && Assembly == other.Assembly && FolderPath == other.FolderPath;
        }

        /// <summary>
        /// Equality conditional operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Item? left, Item? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality conditional operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Item? left, Item? right) => !(left == right);

        /// <summary>
        /// Type agnostic equality test
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
            => obj is Item other && Equals(other);
        

        /// <summary>
        /// Get hash 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCode.Combine(FullPath, Assembly, FolderPath);


        /// <summary>
        /// Presents the Item as a WebView2.Source Uri
        /// NOTE: If Item is a .zip, .tar, .tag.gz, or .tgz file, it will unpackage the file and return the default HTML file 
        /// </summary>
        /// <param name="searchPatterns">files names, searched in package, to be HTML source.  Default: ["index.html", "default.html", "index.htm", "default.htm", "*.html", "*.htm"]</param>
        /// <returns></returns>
        internal async Task<Uri?> AsWebViewSourceAsync(params string[] searchPatterns)
        {
            if (!DirectoryExtensions.IsSupportedPackageExtension(FullPath))
                return FileUri;

            if (await this.UnpackAsync() is not { } dir)
                return null;

            var enumOpt = new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                RecurseSubdirectories = false,
                MaxRecursionDepth = 1
            };

            var priorityFiles = searchPatterns.Length == 0
                ? ["index.html", "default.html", "index.htm", "default.htm", "*.html", "*.htm"]
                : searchPatterns;

            if (PrioritySearch(enumOpt) is { } f0)
                return new Uri(f0.FullName);

            enumOpt.RecurseSubdirectories = true;
            if (PrioritySearch(enumOpt) is { } f1)
                return new Uri(f1.FullName);

            return null;

            FileInfo? PrioritySearch(EnumerationOptions opt)
            {
                foreach (var fileName in priorityFiles)
                    if (dir.GetFiles(fileName, opt).FirstOrDefault() is { } fileInfo)
                        return fileInfo;
                return null;
            }
        }


        #region Locally Stored Value Get/Set

        /// <summary>
        /// Load serialized object from app local storage
        /// </summary>
        /// <param name="defaultValue">optional default value</param>
        /// <param name="serializer">optional System.Text.Json.Serializer</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>defaultValue upon fail</returns>
        // ReSharper disable once MemberCanBeProtected.Global
        public T? RecallValue<T>(T? defaultValue = default, Serializer? serializer = null) 
        {
            if (!this.TryRecallText(out var text) || string.IsNullOrWhiteSpace(text))
                return defaultValue;

            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(text, typeof(T));
            
            serializer ??= Serializer.Default;
            return serializer.Deserialize<T>(text);
        }

        /// <summary>
        /// Load serialized object from app local storage
        /// </summary>
        /// <param name="defaultValue">optional default value</param>
        /// <param name="serializer">optional System.Text.Json.Serializer</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>defaultValue upon fail</returns>
        // ReSharper disable once MemberCanBeProtected.Global
        public async Task<T?> RecallValueAsync<T>(T? defaultValue = default, Serializer? serializer = null)
        {
            var result = await this.TryRecallTextAsync();
            if (!result.success || string.IsNullOrWhiteSpace(result.text))
                return defaultValue;
            
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(result.text, typeof(T));
            
            serializer ??= Serializer.Default;
            return serializer.Deserialize<T>(result.text);
        }


        /// <summary>
        /// Load serialized object from app local storage
        /// </summary>
        /// <param name="result"></param>
        /// <param name="defaultValue">optional default value</param>
        /// <param name="serializer">optional System.Text.Json.Deserializer</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>false on fail</returns>
        public bool TryRecallValue<T>(out T? result, T? defaultValue = default, Serializer? serializer = null)
        {
            result = defaultValue;
            if (!this.TryRecallText(out var text) || string.IsNullOrWhiteSpace(text))
                return false;

            try
            {
                if (typeof(T) == typeof(string))
                {
                    result = (T)Convert.ChangeType(text, typeof(T));
                    return true;
                }
            
                serializer ??= Serializer.Default;
                result = serializer.Deserialize<T>(text);
                return result is not null;
            }
            catch (Exception e)
            {
                QLog.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Load serialized object from app local storage
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="serializer">optional System.Text.Json.Deserializer</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>(success = false, value = default) upon fail</returns>
        // ReSharper disable once MemberCanBeProtected.Global
        public async Task<(bool success, T? value)> TryRecallValueAsync<T>(T? defaultValue = default, Serializer? serializer = null)
        {
            var result = await this.TryRecallTextAsync();
            if (!result.success || string.IsNullOrWhiteSpace(result.text))
                return (false, defaultValue);
            try
            {
                if (typeof(T) == typeof(string))
                {
                    var val = (T)Convert.ChangeType(result.text, typeof(T));
                    return (true, val);
                }
                
                serializer ??= Serializer.Default;
                var value = serializer.Deserialize<T>(result.text);
                return (true, value);
            }
            catch (Exception)
            {
                return (false, defaultValue);
            }
            
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>true on success</returns>
        public void StoreValue<T>(T? obj)
        {
            switch (obj)
            {
                case Item:
                    throw new ArgumentException("Cannot serialize a LocalData.Item");
                case string text:
                    this.StoreText(text);
                    return;
                default:
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(obj);
                    this.StoreText(json);
                    break;
                }
            }
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public async Task StoreValueAsync<T>(T? obj)
        {
            switch (obj)
            {
                case Item:
                    throw new ArgumentException("Cannot serialize a LocalData.Item");
                case string text:
                    await this.StoreTextAsync(text);
                    return;
                default:
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(obj);
                    await this.StoreTextAsync(json);
                    break;
                }
            }
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryStoreValue<T>(T obj)
        {
            if (obj is Item)
                throw new ArgumentException("Cannot serialize a LocalData.Item");
            try
            {
                StoreValue(obj);
                return true;
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>true on success</returns>
        public async Task<bool> TryStoreValueAsync<T>(T obj)
        {
            if (obj is Item)
                throw new ArgumentException("Cannot serialize a LocalData.Item");
            try
            {
                await StoreValueAsync(obj);
                return true;
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
                return false;
            }
        }

        #endregion

    }

    
    /// <summary>
    /// Item referenced by Tag, folder (can be empty string), and Assembly
    /// </summary>
    public class TagItem : Item
    {
        
        /// <summary>
        /// Tag (or key) for item reference
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public string Tag { get; }

        /// <summary>
        /// Instance factory
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="folderPath"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TagItem For(string tag, string folderPath, Assembly assembly)
            => string.IsNullOrEmpty(tag) 
                ? throw new ArgumentNullException(nameof(tag)) 
                : InternalFor(tag, folderPath, assembly);
        
        /// <summary>
        /// Instance Factory
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TagItem For(string tag, Assembly assembly)
            => string.IsNullOrEmpty(tag)
                ? throw new ArgumentNullException(nameof(tag))
                : InternalFor(tag, null, assembly);

        internal static TagItem InternalFor(string tag, string? folderPath = null, Assembly? assembly = null) // `Assembly? assembly = null` is here to facility Clear() being applied to everything.
        {
            if (!string.IsNullOrEmpty(tag))
                tag = CleanKey(tag);
            var rootPath = FullPathForFolderAndAssembly(folderPath, assembly);
            if (!DirectoryExtensions.TryGetOrCreateDirectory(rootPath, out var rootDirectory))
                throw new Exception($"Cannot create folder [{rootPath}] for P42.Utils.LocalData");

            if (string.IsNullOrWhiteSpace(rootDirectory.FullName))
                throw new Exception($"Cannot get rootDirectory.FullName for rootPath [{rootPath}] in P42.Utils.LocalData");

            var fullPath = Path.Combine(rootPath, tag);
            //if (fullPath.Length >= 260)
            //    throw new Exception($"Path [{fullPath}] too long [{fullPath.Length}] for P42.Utils.LocalData");

            return new TagItem(tag, fullPath, folderPath, assembly);

        }


        private TagItem(string tag, string fullPath, string? folderPath, Assembly? assembly) : base(fullPath, folderPath, assembly)
        {
            Tag = tag;
        }

        /// <summary>
        /// String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"Tag: {Tag}; {base.ToString()}";


        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Item? other)
            => other is TagItem && base.Equals(other);
        
        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCode.Combine(Tag, base.GetHashCode());
        
    }


    /// <summary>
    /// Item that comes from a source that can only be pulled asynchronously (ex: internet)
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="folderPath"></param>
    /// <param name="assembly"></param>
    public abstract class AsynchronousSourcedItem(string fullPath, string? folderPath, Assembly assembly) : Item(fullPath, folderPath, assembly)
    {
        /// <summary>
        /// What is the created date / time of the source?
        /// </summary>
        /// <returns></returns>
        protected abstract Task<(bool success, DateTime dateTime)> TrySourceDateTimeAsync();
        
        

        /// <summary>
        /// Is newer source available?
        /// </summary>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public async Task<bool> IsFresherSourceAvailableAsync()
        {
            var result = await TrySourceDateTimeAsync();
            if (!result.success)
                return false; 
            if (IsFile && result.dateTime <= File.GetLastWriteTimeUtc(FullPath) )
                return false;

            return !IsDirectory || result.dateTime > Directory.GetLastWriteTimeUtc(FullPath);
        }

        /// <summary>
        /// Overwrite local value with source's value
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public async Task ResetFromSourceAsync()
        {
            var result = await TrySourceDateTimeAsync();
            if (!result.success)
                return; 

            var bytes = await GetSourceAsBytesAsync();
            await Semaphore.WaitAsync();
            try
            {
                AssureExistsParentDirectory();
                await File.WriteAllBytesAsync(FullPath, bytes);
                File.SetLastAccessTimeUtc(FullPath, result.dateTime);
            }
            finally
            {
                Semaphore.Release();
            }
            
        }

        /// <summary>
        /// Try to overwrite local value with source's value
        /// </summary>
        /// <returns>false upon fail</returns>
        [JetBrains.Annotations.PublicAPI]
        public async Task<bool> TryResetFromSourceAsync()
        {
            var result = await TrySourceDateTimeAsync();
            if (!result.success)
                return false; 

            try
            {
                await ResetFromSourceAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Update local value using source's value IF source value is newer or local value is missing
        /// </summary>
        public async Task RefreshAsync()
        {
            if (!await IsFresherSourceAvailableAsync())
                return;
            await ResetFromSourceAsync();
        }

        /// <summary>
        /// Try to update local value using source's value IF source value is newer or local value is missing
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryRefreshAsync()
        {
            if (!await IsFresherSourceAvailableAsync())
                return false; 
            try
            {
                await RefreshAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// use local value, if exists, else use source value
        /// </summary>
        public async Task AssureExistsAsync()
        {
            if (IsFile)
                return;
            await ResetFromSourceAsync();
        }

        /// <summary>
        /// use local value, if exists, else use source value
        /// </summary>
        /// <returns>true on success</returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public async Task<bool> TryAssureExistsAsync()
            => IsFile || await TryResetFromSourceAsync();
        
        /// <summary>
        /// get local value or source value (if local does not exist or source is newer)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>local value or source value (when local value !exist or source value is newer)</returns>
        public async Task<T?> GetFreshValueAsync<T>(T? defaultValue = default)
        {
            await TryRefreshAsync();
            return await RecallValueAsync(defaultValue);
        }

        /// <summary>
        /// try to get local value or source value (if local does not exist or source is newer)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>(success, value) where value = defaultValue when success = false</returns>
        public async Task<(bool success, T? value)> TryGetFreshValueAsync<T>(T? defaultValue = default)
        {
            await TryRefreshAsync();
            var result = await TryRecallValueAsync<T>();
            return result.success 
                ? result 
                : (false, defaultValue);
        }

        /// <summary>
        /// get local value or source value (if local does not exist)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T?> GeAssuredValueAsync<T>(T? defaultValue = default)
        {
            await TryAssureExistsAsync();
            return await RecallValueAsync(defaultValue);
        }

        /// <summary>
        /// get local value or source value (if local does not exist)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>local value or source value (when local value !exist)</returns>
        public async Task<(bool success, T? value)> TryGetAssuredValueAsync<T>(T? defaultValue = default)
        {
            await TryAssureExistsAsync();
            var result = await TryRecallValueAsync(defaultValue);
            return result.success 
                ? result 
                : (false, defaultValue);
        }

        /// <summary>
        ///  Value stored at source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>value</returns>
        public async Task<T?> GetSourceValueAsync<T>(T? defaultValue = default)
        {
            var result = await TrySourceDateTimeAsync();
            if (!result.success)
                throw new SourceNotAvailableException(ToString());

            var bytes = await GetSourceAsBytesAsync();
            if (bytes.Length == 0)
                return defaultValue;

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(json))
                return defaultValue;

            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(json, typeof(T));

            var value = System.Text.Json.JsonSerializer.Deserialize<T>(json);
            return value;
        }

        /// <summary>
        /// Try to get value stored at source
        /// </summary>
        /// <param name="defaultValue">optional default value upon failure</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>(success, value) where value = defaultValue when success=false</returns>
        public async Task<(bool success, T? value)> TryGetSourceValueAsync<T>(T? defaultValue = default)
        {
            var result = await TrySourceDateTimeAsync();
            if (!result.success)
                return (false, defaultValue);

            try
            {
                var bytes = await GetSourceAsBytesAsync();
                if (bytes.Length == 0)
                    return (true, defaultValue);

                var json = System.Text.Encoding.UTF8.GetString(bytes);
                if (string.IsNullOrEmpty(json))
                    return (true, defaultValue);

                if (typeof(T) == typeof(string))
                {
                    var val = (T)Convert.ChangeType(json, typeof(T));
                    return (true, val);
                }
                
                var value = System.Text.Json.JsonSerializer.Deserialize<T>(json);
                return (true, value);
            }
            catch (Exception e)
            {
                QLog.Warning(e);
                return (false, defaultValue);
            }
        }
        
        protected abstract Task<byte[]> GetSourceAsBytesAsync();
    }


    /// <summary>
    /// Item that comes from a source that can be pulled synchronously or asynchronously (ex: EmbeddedResource)
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="folderPath"></param>
    /// <param name="assembly"></param>
    public abstract class SynchronousSourcedItem(string fullPath, string? folderPath, Assembly assembly) : AsynchronousSourcedItem(fullPath, folderPath, assembly)
    {
        /// <summary>
        /// What is the created date / time of the source?
        /// </summary>
        /// <returns></returns>
        protected abstract bool TrySourceDateTime(out DateTime dateTime);

        /// <summary>
        /// Is newer source available?
        /// </summary>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public bool IsFresherSourceAvailable()
        {
            if (!TrySourceDateTime(out DateTime sourceDate))
                return false; 
            
            if (IsFile && sourceDate <= File.GetLastWriteTimeUtc(FullPath) )
                return false;

            return !IsDirectory || sourceDate > Directory.GetLastWriteTimeUtc(FullPath);
        }

        /// <summary>
        /// Overwrite local value with source's value
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public void ResetFromSource()
        {
            if (!TrySourceDateTime(out DateTime buildDate))
                return; 

            var bytes = GetSourceAsBytes();
            Semaphore.Wait();
            try
            {
                AssureExistsParentDirectory();
                File.WriteAllBytes(FullPath, bytes);
                File.SetLastAccessTimeUtc(FullPath, buildDate);
            }
            finally
            {
                Semaphore.Release();
            }

        }
        
        /// <summary>
        /// Try to overwrite local value with source's value
        /// </summary>
        /// <returns>false upon fail</returns>
        [JetBrains.Annotations.PublicAPI]
        public bool TryResetFromSource()
        {
            if (!TrySourceDateTime(out _))
                return false;

            try
            {
                ResetFromSource();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Update local value using source's value IF source value is newer or local value is missing
        /// </summary>
        public void Refresh()
        {
            if (!TrySourceDateTime(out DateTime sourceDate))
                return; 

            if (IsFile && sourceDate <= File.GetLastWriteTimeUtc(FullPath) )
                return;

            if (IsDirectory && sourceDate <= Directory.GetLastWriteTimeUtc(FullPath))
                return;

            ResetFromSource();
        }

        /// <summary>
        /// Try to update local value using source's value IF source value is newer or local value is missing
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        [JetBrains.Annotations.PublicAPI]
        public bool TryRefresh()
        {
            if (!IsFresherSourceAvailable())
                return false; 
            try
            {
                Refresh();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// use local value, if exists, else use source value
        /// </summary>
        public void AssureExists()
        {
            if (IsFile)
                return;
            ResetFromSource();
        }

        /// <summary>
        /// use local value, if exists, else use source value
        /// </summary>
        /// <returns>true on success</returns>
        public bool TryAssureExists()
            => IsFile || TryResetFromSource();
        
        
        /// <summary>
        /// get local value or source value (if local does not exist or source is newer)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>local value or source value (when local value !exist or source value is newer)</returns>
        public T? GetFreshValue<T>(T? defaultValue = default)
        {
            TryRefresh();
            return RecallValue(defaultValue);
        }

        /// <summary>
        /// try to get local value or source value (if local does not exist or source is newer)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">optional</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>local value or source value (when local value !exist or source value is newer)</returns>
        public bool TryGetFreshValue<T>(out T? value, T? defaultValue = default)
        {
            TryRefresh();
            return TryRecallValue(out value, defaultValue);
        }
    
        /// <summary>
        /// get local value or source value (if local does not exist)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GeAssuredValue<T>(T? defaultValue = default)
        {
            TryAssureExists();
            return RecallValue(defaultValue);
        }

        /// <summary>
        /// get local value or source value (if local does not exist)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>local value or source value (when local value !exist)</returns>
        public bool TryGetAssuredValue<T>(out T? value, T? defaultValue = default)
        {
            TryAssureExists();
            return TryRecallValue(out value, defaultValue);
        }

        /// <summary>
        ///  Value stored at source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>source value</returns>
        public T? GetSourceValue<T>(T? defaultValue = default)
        {
            if (!TrySourceDateTime(out _))
                return defaultValue;

            var bytes = GetSourceAsBytes();
            if (bytes.Length == 0)
                return defaultValue;

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(json))
                return defaultValue;

            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(json, typeof(T));

            var value = System.Text.Json.JsonSerializer.Deserialize<T>(json);
            return value;
        }

        /// <summary>
        ///  Try to get value stored at source
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">optional default value</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetSourceValue<T>(out T? value, T? defaultValue = default)
        {
            value = defaultValue;
            if (!TrySourceDateTime(out _))
                return false;

            try
            {
                var bytes = GetSourceAsBytes();
                if (bytes.Length == 0)
                    return true;

                var json = System.Text.Encoding.UTF8.GetString(bytes);
                if (string.IsNullOrEmpty(json))
                    return true;

                if (typeof(T) == typeof(string))
                {
                    value = (T)Convert.ChangeType(json, typeof(T));
                    return true;
                }
                
                value = System.Text.Json.JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                QLog.Warning(ex);
                return false;
            }
        }
        
        protected abstract byte[] GetSourceAsBytes();
    }


    /// <summary>
    /// Tage referencing a remotely hosted file
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class UriItem : AsynchronousSourcedItem
    {
        private static readonly ObservableConcurrentDictionary<string, string> ItemUriRootLookup;
        protected static readonly ObservableConcurrentDictionary<string, string> ETagLookup;
        protected static readonly ObservableConcurrentDictionary<string, DateTime> ETagDates;

        private static readonly string ItemUriRootLookupPath = Path.Combine(StorePath, nameof(ItemUriRootLookup));
        private static readonly string ETagLookupPath = Path.Combine(StorePath, nameof(ETagLookup));
        private static readonly string ETagDatesPath = Path.Combine(StorePath, nameof(ETagDates));

        static UriItem()
        {
            Serializer.Default.Add(typeof(ObservableConcurrentDictionary<string, string>), ObservableConcurrentDictionary_string_string_SerializerContext.Default);
            Serializer.Default.Add(typeof(ObservableConcurrentDictionary<string, DateTime>), ObservableConcurrentDictionary_string_DateTime_SerializerContext.Default);

            if (File.Exists(ItemUriRootLookupPath) &&
                File.ReadAllText(ItemUriRootLookupPath) is { } urJson &&
                !string.IsNullOrWhiteSpace(urJson) &&
                //JsonConvert.DeserializeObject<ObservableConcurrentDictionary<string, string>>(urJson) is { } uriLookup)
                Serializer.Default.TryDeserialize<ObservableConcurrentDictionary<string, string>>(urJson, out var uriLookup))
                ItemUriRootLookup = uriLookup;
            else
                ItemUriRootLookup = new ObservableConcurrentDictionary<string, string>();
            
            if (File.Exists(ETagLookupPath) &&
                File.ReadAllText(ETagLookupPath) is { } etagJson &&
                !string.IsNullOrWhiteSpace(etagJson) &&
                //JsonConvert.DeserializeObject<ObservableConcurrentDictionary<string, string>>(etagJson) is { } etagLookup)
                Serializer.Default.TryDeserialize<ObservableConcurrentDictionary<string, string>>(etagJson, out var etagLookup))
                ETagLookup = etagLookup;
            else
                ETagLookup = new ObservableConcurrentDictionary<string, string>();

            if (File.Exists(ETagDatesPath) &&
                File.ReadAllText(ETagDatesPath) is { } etagDatesJson &&
                !string.IsNullOrWhiteSpace(etagDatesJson) &&
                //JsonConvert.DeserializeObject<ObservableConcurrentDictionary<string, string>>(etagJson) is { } etagLookup)
                Serializer.Default.TryDeserialize<ObservableConcurrentDictionary<string, DateTime>>(etagDatesJson, out var etagDates))
                ETagDates = etagDates;
            else
                ETagDates = new ObservableConcurrentDictionary<string, DateTime>();

            ItemUriRootLookup.CollectionChanged += OnItemUriRootLookupCollectionChanged;
            ETagLookup.CollectionChanged += OnETagLookupCollectionChanged;
            ETagDates.CollectionChanged += OnETagDatesCollectionChanged;
        }
        
        private static async void OnETagDatesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                var dateJson = System.Text.Json.JsonSerializer.Serialize(ETagDates);
                await Semaphore.WaitAsync();
                try
                {
                    DirectoryExtensions.GetOrCreateParentDirectory(ETagDatesPath);
                    await File.WriteAllTextAsync(ETagDatesPath, dateJson);
                }
                catch (Exception ex)
                {
                    QLog.Error(ex);
                }
                finally
                {
                    Semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }
        }

        // use of async void is ok here since we're handling exceptions
        private static async void OnETagLookupCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                //var json = JsonConvert.SerializeObject(ETagLookup);
                var etagJson = System.Text.Json.JsonSerializer.Serialize(ETagLookup);
                await Semaphore.WaitAsync();
                try
                {
                    DirectoryExtensions.GetOrCreateParentDirectory(ETagLookupPath);
                    await File.WriteAllTextAsync(ETagLookupPath, etagJson);
                }
                catch (Exception ex)
                {
                    QLog.Error(ex);
                }
                finally
                {
                    Semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }
        }

        // use of async void is ok here since we're handling exceptions
        private static async void OnItemUriRootLookupCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                //var json = JsonConvert.SerializeObject(ItemUriRootLookup);
                var json = System.Text.Json.JsonSerializer.Serialize(ItemUriRootLookup);
                await Semaphore.WaitAsync();
                try
                {
                    DirectoryExtensions.GetOrCreateParentDirectory(ItemUriRootLookupPath);
                    await File.WriteAllTextAsync(ItemUriRootLookupPath, json);
                }
                catch (Exception ex)
                {
                    QLog.Error(ex);
                }
                finally
                {
                    Semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }
        }
            
        /// <summary>
        /// Uri for Source
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public Uri SourceUri { get; }

        /// <summary>
        /// Root uri for source (in the case of directory sources)
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public Uri? RootUri { get; }

        /// <summary>
        /// ???
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string LocalPath { get; }

        /// <summary>
        /// Convert a Uri into an item key (replacing rootUri portion with a Guid to prevent naming conflicts)
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="rootUri">part of Uri replaced with Guid (default: "scheme://host")</param>
        /// <param name="folderPath"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static UriItem For(Uri sourceUri, Uri? rootUri = null, string? folderPath = null, Assembly? assembly = null)
        {
            assembly ??= AssemblyExtensions.GetApplicationAssembly();

            var root = HttpUtility.UrlDecode(sourceUri.Host);
            var storeSubPath = HttpUtility.UrlDecode(sourceUri.LocalPath);

            if (rootUri is not null)
            {
                if (!sourceUri.AbsoluteUri.StartsWith(rootUri.AbsoluteUri))
                    throw new Exception(
                        $"SourceUri [{sourceUri}] does not start with RootUri [{rootUri}]for P42.Utils.LocalData");
                root = HttpUtility.UrlDecode(rootUri.AbsoluteUri);
                var relativeUri = sourceUri.AbsoluteUri[rootUri.AbsoluteUri.Length..];
                storeSubPath = HttpUtility.UrlDecode(relativeUri);
            }

            var rootPath = RootFolderPath(root, folderPath, assembly);
            var fullPath = Path.Combine(rootPath, storeSubPath);
            //if (fullPath.Length >= 260)
            //    throw new Exception($"Path [{fullPath}] too long [{fullPath.Length}] for P42.Utils.LocalData");

            var parentPath = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrWhiteSpace(parentPath))
                throw new Exception($"Cannot get path for parent of {fullPath}");
            
            if (!DirectoryExtensions.TryGetOrCreateDirectory(parentPath, out var itemParent, true))
                throw new Exception($"Cannot create folder [{parentPath}] for P42.Utils.LocalData");

            if (itemParent.FullName is null)
                throw new Exception($"Cannot get itemParent.FullName for [{fullPath}] in P42.Utils.LocalData");

            return new UriItem(sourceUri, rootUri, storeSubPath, fullPath, folderPath, assembly);
        }

        /// <summary>
        /// Convert a Uri into an item key (replacing rootUri portion with a Guid to prevent naming conflicts)
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static UriItem For(Uri sourceUri, Assembly assembly)
            => For(sourceUri, null, null, assembly);
        
        private UriItem(Uri sourceUri, Uri? rootUri, string localPath, string fullPath, string? folderPath, Assembly assembly) : base(fullPath,
            folderPath, assembly)
        {
            SourceUri = sourceUri;
            RootUri = rootUri;
            LocalPath = localPath;
        }

        private static string RootFolderPath(string root, string? folderPath, Assembly assembly)
        {
            if (ItemUriRootLookup.TryGetValue(root, out var itemGuid) && !string.IsNullOrWhiteSpace(itemGuid))
                return Path.Combine(FullPathForFolderAndAssembly(folderPath, assembly), itemGuid);

            itemGuid = Guid.NewGuid().ToString();
            ItemUriRootLookup[root] = itemGuid;
            return Path.Combine(FullPathForFolderAndAssembly(folderPath, assembly), itemGuid);
        }

        /// <summary>
        /// Human-readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"SourceUri: {SourceUri}; RootUri: {RootUri}; {base.ToString()}";
        
        
        private static bool TryGetLastModified(HttpResponseMessage response, out DateTime lastModified)
        {
            if (response.Headers.TryGetValues("Last-Modified", out var headers) 
                                && headers.FirstOrDefault() is { } lastModifiedString 
                                && DateTime.TryParseExact(lastModifiedString, "R", CultureInfo.InvariantCulture,
                                    DateTimeStyles.AssumeUniversal, out lastModified))
                                return true;
            
            lastModified = DateTime.MinValue;
            return false;
        }

        private static bool TryGetEtag(HttpResponseMessage response, out string? etag)
        {
            if (response.Headers.ETag is { } sourceETag)
            {
                etag = sourceETag.Tag;
                return true;
            }
            
            etag = null;
            return false;
        }
        
        protected override async Task<(bool success, DateTime dateTime)> TrySourceDateTimeAsync()
        {
            try
            {
                if (!ETagLookup.TryGetValue(FullPath, out var storedETag))
                    storedETag = string.Empty;

                if (!ETagDates.TryGetValue(storedETag, out var storedETagDate))
                    storedETagDate = SessionDateTime;
                
                // Get the version / time-stamp headers
                using var headerRequest = new HttpRequestMessage(HttpMethod.Head, SourceUri);
                using var headerResponse = await HttpClient.SendAsync(headerRequest);

                if (!headerResponse.IsSuccessStatusCode)
                    return (false, DateTime.MinValue);

                if (!TryGetEtag(headerResponse, out var sourceETag))
                    sourceETag = string.Empty;
                
                // use lastModified to version source?
                if (TryGetLastModified(headerResponse, out var lastModified))
                {
                    if (string.IsNullOrEmpty(sourceETag))
                        return (true, lastModified);
                    
                    ETagDates.Remove(storedETag);
                    ETagLookup[FullPath] = sourceETag;
                    ETagDates[sourceETag] = lastModified;
                    return (true,lastModified);
                }

                // there is no ability to version the source
                if (string.IsNullOrEmpty(sourceETag))
                    return (true, SessionDateTime);

                // use ETag to version source
                ETagLookup[FullPath] = sourceETag;
                ETagDates[sourceETag] = storedETagDate;
                return (true, storedETagDate);
            }
            catch (Exception e)
            {
                QLog.Warning(e);
                return (false, SessionDateTime);
            }
        }

        protected override Task<byte[]> GetSourceAsBytesAsync()
            => HttpClient.GetByteArrayAsync(SourceUri);

        public override bool Equals(Item? other)
            => other is UriItem && base.Equals(other);
        

        public override int GetHashCode()
            => HashCode.Combine(SourceUri, RootUri, base.GetHashCode());

        
    }
    
    
    /// <summary>
    /// Tag referencing an EmbeddedResource 
    /// </summary>
    // ReSharper disable once UnusedType.Global
    public class ResourceItem : SynchronousSourcedItem
    {
        /// <summary>
        /// ResourceId for Embedded Resource Source
        /// </summary>
        [JetBrains.Annotations.PublicAPI]
        public string ResourceId { get; }
        
        /// <summary>
        /// Get ResourceItem for Embedded Resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="folderPath"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static ResourceItem For(string resourceId, string? folderPath = null, Assembly? assembly = null) 
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentOutOfRangeException(nameof(resourceId));

            resourceId = CleanKey(resourceId);
            var givenAssembly = assembly;
            var eResource = EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(resourceId, givenAssembly);

            if (eResource is null)
                throw new ArgumentException( $"Cannot find resourceId [{resourceId}] in provided assembly [{givenAssembly?.Name() ?? "null"}]");

            resourceId = eResource.ResourceId;
            assembly = eResource.Assembly;
            eResource.DisposableStream.Dispose();

            var rootPath = FullPathForFolderAndAssembly(folderPath, assembly);
            if (!DirectoryExtensions.TryGetOrCreateDirectory(rootPath, out var rootDirectory))
            {
                eResource.DisposableStream.Dispose();
                throw new Exception($"Cannot create folder [{rootPath}] for P42.Utils.LocalData");
            }

            if (string.IsNullOrWhiteSpace(rootDirectory.FullName))
            {
                eResource.DisposableStream.Dispose();
                throw new Exception($"Cannot get rootDirectory.FullName for rootPath [{rootPath}] in P42.Utils.LocalData");
            }

            var fullPath = Path.Combine(rootPath, resourceId);
            //if (fullPath.Length >= 260)
            //    throw new Exception($"Path [{fullPath}] too long [{fullPath.Length}] for P42.Utils.LocalData");
            
            return new ResourceItem(resourceId, fullPath, folderPath, assembly);
        }

        /// <summary>
        /// Get ResourceItem for Embedded Resource
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ResourceItem For(string resourceId, Assembly assembly)
            => For(resourceId, null, assembly);
        
        private ResourceItem(string resourceId, string fullPath, string? folderPath, Assembly assembly) : 
            base(fullPath, folderPath, assembly)
        {
            ResourceId = resourceId;
        }

        /// <summary>
        /// Human-readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"Resource: {ResourceId}; {base.ToString()}";
        
        /// <summary>
        /// What is the created date / time of the source?
        /// </summary>
        /// <returns></returns>
        protected override async Task<(bool success, DateTime dateTime)> TrySourceDateTimeAsync()
            => (true, await Assembly.GetBuildTimeAsync());


        /// <summary>
        /// What is the created date / time of the source?
        /// </summary>
        /// <returns></returns>
        protected override bool TrySourceDateTime(out DateTime dateTime)
        {
            dateTime = Assembly.GetBuildTime();
            return true;
        } 

        /// <summary>
        /// Equality method
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Item? other)
            => other is ResourceItem && base.Equals(other);
        
        /// <summary>
        /// Hashcode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => HashCode.Combine(ResourceId, base.GetHashCode());

        protected override byte[] GetSourceAsBytes()
        {
            if (EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(ResourceId, Assembly) is not {} resource)
                throw new Exception($"EmbeddedResource not found for LocalData.ResourceItem [{this}]");

            try
            {
                using var stream = new MemoryStream();
                resource.DisposableStream.CopyTo(stream);
                return stream.ToArray();
            }
            finally
            {
                resource.Dispose();
            }
        }

        protected override async Task<byte[]> GetSourceAsBytesAsync()
        {
            if (EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(ResourceId, Assembly) is not {} resource)
                throw new Exception($"EmbeddedResource not found for LocalData.ResourceItem [{this}]");

            try
            {
                using var stream = new MemoryStream();
                await resource.DisposableStream.CopyToAsync(stream);
                return stream.ToArray();
            }
            finally
            {
                await resource.DisposeAsync();
            }
            
        }

    }


    #endregion


    
}

public class SourceNotAvailableException(string sourceTag) : Exception(sourceTag);


