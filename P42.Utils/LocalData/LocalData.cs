using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using P42.Serilog.QuickLog;
using static P42.Utils.LocalData;

namespace P42.Utils;



/// <summary>
/// base for storage/recall of items in local data store for common caching scenarios
/// </summary>
public abstract partial class LocalData
{
    
    #region Fields

    internal static readonly SemaphoreSlim Semaphore = new (1, 1);
    

    /// <summary>
    /// Used to set folder used for local data store.  
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
#pragma warning disable CS0618 // Type or member is obsolete
    // DO NOT CHANGE Environment.ApplicationDataPath to another path.
    // This is used to pass EmbeddedResource Fonts to UWP Text elements and there is zero flexibility here.
    private static string PlatformFolder => Environment.ApplicationLocalFolderPath;
#pragma warning restore CS0618 // Type or member is obsolete
    private const string StoreFolderName = "P42.Utils.LocalData";
    private static readonly string StorePath = Path.Combine(PlatformFolder, StoreFolderName);
    private static readonly string ItemUriRootLookupPath = Path.Combine(StorePath, nameof(ItemUriRootLookup));
    private static readonly string ETagLookupPath = Path.Combine(StorePath, nameof(ETagLookup));

    // ReSharper disable StaticMemberInGenericType
    private static readonly ObservableConcurrentDictionary<string, string> ItemUriRootLookup;
    protected static readonly ObservableConcurrentDictionary<string, string> ETagLookup;


    protected static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> ItemLoadingTasks = new();
    protected static readonly HttpClient HttpClient = new();
    // ReSharper restore StaticMemberInGenericType

    protected static DateTime VersionDateTime = DateTime.MinValue;

    #endregion

    
    #region Construction / Initialization

    static LocalData()
    {
        //TODO: Right now, the local data store is cleared when the app is updated.  This may not be the best behavior for downloaded items.

        if (!DirectoryExtensions.TryGetOrCreateDirectory(PlatformFolder, out var platformFolder))
            throw new Exception($"Cannot create PlatformFolder [{PlatformFolder}] for P42.Utils.LocalData");

        if (platformFolder.FullName is null)
            throw new Exception($"Cannot get PlatformFolder.FullName [{PlatformFolder}] for P42.Utils.LocalData");

        var asm = AssemblyExtensions.GetApplicationAssembly();
        var version = asm.GetName().Version;
        if (version is null)
            throw new Exception("Cannot get Version for P42.Utils.LocalData");

        var versionFilePath = Path.Combine(platformFolder.FullName, "P42.Utils.LocalData.Version.txt");

        if (!DirectoryExtensions.TryGetOrCreateDirectory(StorePath, out var storeFolder))
            throw new Exception($"Cannot create StoreFolder [{StorePath}] forP42.Utils.LocalData");
        if (storeFolder is null)
            throw new Exception($"Cannot get StoreFolder [{StorePath}] for P42.Utils.LocalData");

        if (File.Exists(versionFilePath))
        {
            var oldVersion = File.ReadAllText(versionFilePath);
            if (!string.IsNullOrWhiteSpace(oldVersion) && oldVersion != version.ToString())
            {
                storeFolder.Delete(true);
                storeFolder.Create();
            }

        }
        else
        {

        }

            DirectoryExtensions.GetOrCreateParentDirectory(versionFilePath);
        File.WriteAllText(versionFilePath, version.ToString());

        if (File.Exists(ItemUriRootLookupPath) &&
            File.ReadAllText(ItemUriRootLookupPath) is { } urJson &&
            !string.IsNullOrWhiteSpace(urJson) &&
            JsonConvert.DeserializeObject<ObservableConcurrentDictionary<string, string>>(urJson) is { } uriLookup)
            ItemUriRootLookup = uriLookup;
        else
            ItemUriRootLookup = new ObservableConcurrentDictionary<string, string>();
        
        if (File.Exists(ETagLookupPath) &&
            File.ReadAllText(ETagLookupPath) is { } etagJson &&
            !string.IsNullOrWhiteSpace(etagJson) &&
            JsonConvert.DeserializeObject<ObservableConcurrentDictionary<string, string>>(etagJson) is { } etagLookup)
            ETagLookup = etagLookup;
        else
            ETagLookup = new ObservableConcurrentDictionary<string, string>();

        ItemUriRootLookup.CollectionChanged += OnItemUriRootLookupCollectionChanged;
        ETagLookup.CollectionChanged += OnETagLookupCollectionChanged;
    }

    // use of async void is ok here since we're handling exceptions
    private static async void OnETagLookupCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            var json = JsonConvert.SerializeObject(ETagLookup);
            await Semaphore.WaitAsync();
            try
            {
                DirectoryExtensions.GetOrCreateParentDirectory(ETagLookupPath);
                await File.WriteAllTextAsync(ETagLookupPath, json);
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
            var json = JsonConvert.SerializeObject(ItemUriRootLookup);
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

        if (Directory.Exists(key.FullPath))
            return Directory.EnumerateFiles(key.FullPath, "*", SearchOption.AllDirectories).ToList();
        
        return [];
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
        public Assembly? Assembly { get; } = assembly;

        /// <summary>
        /// FolderPath (used to further compartmentalize items (default: null)
        /// </summary>
        public string FolderPath { get; } = folderPath?.Trim('/').Trim('\\') ?? string.Empty;

        /// <summary>
        /// File system path to item
        /// </summary>
        public string FullPath { get; } = fullPath;

        /// <summary>
        /// "ms-appdata://" path to item
        /// </summary>
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
        public Uri AppDataUri => new Uri(AppDataUrl);

        public Uri FileUri => new Uri(FullPath);

        /// <summary>
        /// Settings for Json Serialization
        /// </summary>
        public JsonSerializerSettings? JsonSerializatingSettings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        /// Settings for Json Deserialization
        /// </summary>
        public JsonSerializerSettings? JsonDeserializingSettings = new ()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Auto
        };



        protected static string CleanKey(string key)
        {
            key = key.Trim();
            if (!key.IsLegalFileName())
                throw new Exception($"Invalid file name characters found in [{key}] for P42.Utils.LocalData");
            return key;
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
            => $"FolderPath: {FolderPath}; Assembly: {Assembly?.Name()??"no assembly"}";

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
                throw new Exception($"Unknown file type for Itemkey [{this}]");
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
        public DirectoryInfo AssureParentDirectory()
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
        {
            if (obj is not Item other)
                return false;
            return Equals(other);
        }

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
        /// <param name="searchPatterns">files names, searched in package, to be html source.  Default: ["index.html", "default.html", "index.htm", "default.htm", "*.html", "*.htm"]</param>
        /// <returns></returns>
        internal async Task<Uri?> AsWebViewSourceAsync(params string[] searchPatterns)
        {
            if (!DirectoryExtensions.IsSupportedPackageExtension(FullPath))
                return FileUri;

            if (await this.UnpackAsync() is not DirectoryInfo dir)
                return null;

            var enumOpt = new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                RecurseSubdirectories = false,
                MaxRecursionDepth = 1
            };

            string[] priorityFiles = searchPatterns.Length == 0
                ? ["index.html", "default.html", "index.htm", "default.htm", "*.html", "*.htm"]
                : searchPatterns;

            FileInfo? PrioritySearch(EnumerationOptions opt)
            {
                foreach (var fileName in priorityFiles)
                    if (dir.GetFiles(fileName, opt)?.FirstOrDefault() is FileInfo fileInfo)
                        return fileInfo;
                return null;
            }

            if (PrioritySearch(enumOpt) is FileInfo f0)
                return new Uri(f0.FullName);

            enumOpt.RecurseSubdirectories = true;
            if (PrioritySearch(enumOpt) is FileInfo f1)
                return new Uri(f1.FullName);

            return null;
        }


        #region Serialize / Deserialize

        /// <summary>
        /// Load serialized object from app local storage
        /// </summary>
        /// <param name="item"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? Deserialize<T>(T? defaultValue = default)
        {
            if (!this.TryRecallText(out var text) || string.IsNullOrWhiteSpace(text))
                return defaultValue;

            return JsonConvert.DeserializeObject<T>(text, JsonDeserializingSettings);
        }

        /// <summary>
        /// Load serialized object from app local storage
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>false on fail</returns>
        public bool TryDeserialize<T>([MaybeNullWhen(false)] out T? result)
        {
            result = default;
            if (!this.TryRecallText(out var text) || string.IsNullOrWhiteSpace(text))
                return false;

            try
            {
                result = JsonConvert.DeserializeObject<T>(text, JsonDeserializingSettings);
                return true;
            }
            catch (Exception e)
            {
                QLog.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="item"></param>
        /// <returns>true on success</returns>
        public void Serialize<T>(T? obj)
        {
            if (obj is Item)
                throw new ArgumentException($"Cannot serialize a LocalData.Item");
            var json = JsonConvert.SerializeObject(obj, JsonSerializatingSettings);
            this.StoreText(json);
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task SerializeAsync<T>(T? obj)
        {
            if (obj is Item)
                throw new ArgumentException($"Cannot serialize a LocalData.Item");
            var json = JsonConvert.SerializeObject(obj, JsonSerializatingSettings);
            await this.StoreTextAsync(json);
        }

        /// <summary>
        /// Attempt to serialize object to app local storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TrySerialize<T>(T obj)
        {
            if (obj is Item)
                throw new ArgumentException($"Cannot serialize a LocalData.Item");
            try
            {
                Serialize(obj);
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
        /// <param name="item"></param>
        /// <returns>true on success</returns>
        public async Task<bool> TrySerializeAsync<T>(T obj)
        {
            if (obj is Item)
                throw new ArgumentException($"Cannot serialize a LocalData.Item");
            try
            {
                await SerializeAsync(obj);
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
        public string Tag { get; }

        public static TagItem Get(string tag, string folderPath, Assembly assembly)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentNullException(nameof(tag));
            return InternalGet(tag, folderPath, assembly);
        }

        internal static TagItem InternalGet(string tag, string folderPath, Assembly? assembly = null) // `Assembly? assembly = null` is here to facility Clear() being applied to everything.
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


        private TagItem(string tag, string fullPath, string folderPath, Assembly? assembly) : base(fullPath, folderPath, assembly)
        {
            Tag = tag;
        }

        public override string ToString()
            => $"Tag: {Tag}; {base.ToString()}";


        public override bool Equals(Item? other)
        {
            if (other is not TagItem)
                return false;

            return base.Equals(other);
        }

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
        /// Is the item available from the source?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public abstract Task<bool> IsSourceAvailableAsync();


        public abstract Task ForcePullAsync();

        /// <summary>
        /// Attempt to pull the item from source
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> TryForcePullAsync()
        {
            try
            {
                await ForcePullAsync();
                return true;
            }
            catch (Exception) { }

            return false;
        }

        /// <summary>
        /// Attempt to recall or pull the item from source
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> TryAssurePulledAsync()
        {
            if (Exists)
                return true;

            return await TryForcePullAsync();
        }

        /// <summary>
        /// Deserializes item from local data store unless not there or ItemKey.Source is more up to date.  Then, first pulls item from ItemKey source before deserializing.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T?> AssureAndDeserializeAsync<T>(T? defaultValue = default)
        {
            return await this.AssureSourcedTextAsync() is { }
                ? Deserialize(defaultValue)
                : defaultValue;
        }


    }


    /// <summary>
    /// Item that comes from a source that can be pulled sychronously or asynchronously (ex: EmbeddedResource)
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="folderPath"></param>
    /// <param name="assembly"></param>
    public abstract class SynchronousSourcedItem(string fullPath, string? folderPath, Assembly assembly) : AsynchronousSourcedItem(fullPath, folderPath, assembly)
    {
        /// <summary>
        /// Is the item available from the source?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public abstract bool IsSourceAvailable();


        public abstract void ForcePull();

        /// <summary>
        /// Attempt to pull the item from source
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool TryForcePull()
        {
            try
            {
                ForcePull();
                return true;
            }
            catch (Exception) { }

            return false;
        }

        /// <summary>
        /// Attempt to recall or pull the item from source
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool TryAssurePulled()
        {
            if (Exists)
                return true;

            return TryForcePull();
        }

        /// <summary>
        /// Deserializes item from local data store unless not there or ItemKey.Source is more up to date.  Then, first pulls item from ItemKey source before deserializing.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? AssureAndDeserialize<T>(T? defaultValue = default)
        {
            return this.AssureSourcedText() is { }
                ? Deserialize(defaultValue)
                : defaultValue;
        }


    }


    /// <summary>
    /// Tag referencing an EmbeddedResource 
    /// </summary>
    public class ResourceItem : SynchronousSourcedItem
    {
        public string ResourceId { get; }
        
        public static ResourceItem Get(string resourceId, string? folderPath = null, Assembly? assembly = null) 
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
                throw new Exception($"Cannot create folder [{rootPath}] for P42.Utils.LocalData");

            if (string.IsNullOrWhiteSpace(rootDirectory.FullName))
                throw new Exception($"Cannot get rootDirectory.FullName for rootPath [{rootPath}] in P42.Utils.LocalData");

            var fullPath = Path.Combine(rootPath, resourceId);
            //if (fullPath.Length >= 260)
            //    throw new Exception($"Path [{fullPath}] too long [{fullPath.Length}] for P42.Utils.LocalData");

            return new ResourceItem(resourceId, fullPath, folderPath, assembly);
        }

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
        /// Is the item available from the source?
        /// </summary>
        /// <returns></returns>
        public override Task<bool> IsSourceAvailableAsync() => Task.FromResult(IsSourceAvailable());

        /// <summary>
        /// Is the item available from the source
        /// </summary>
        /// <returns></returns>
        public override bool IsSourceAvailable() => Assembly?.Exists(ResourceId) ?? false;

        /// <summary>
        /// Pull the resource asynchronously
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public override async Task ForcePullAsync()
        {

            if (Assembly is null) throw new ArgumentNullException(nameof(Assembly));    

            if (EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(ResourceId, Assembly) is not {} resource)
                throw new Exception($"EmbeddedResource not found for LocalData.ResourceItem [{this}]");

            await Semaphore.WaitAsync();

            try
            {
                await using var destinationStream = new FileStream(FullPath, FileMode.Create);
                await resource.DisposableStream.CopyToAsync(destinationStream);

                if (Assembly.TryGetBuildTime(out var buildDateTime))
                    File.SetLastWriteTime(FullPath, buildDateTime);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Semaphore.Release();
            }

            await resource.DisposableStream.DisposeAsync();
        }

        public override void ForcePull()
        {
            if (Assembly is null) throw new ArgumentNullException(nameof(Assembly));

            if (EmbeddedResourceExtensions.FindAssemblyResourceIdAndStream(ResourceId, Assembly) is not { } resource)
                throw new Exception($"EmbeddedResource not found for LocalData.ResourceItem [{this}]");

            Semaphore.Wait();

            try
            {
                using var destinationStream = new FileStream(FullPath, FileMode.Create);
                resource.DisposableStream.CopyTo(destinationStream);

                if (Assembly.TryGetBuildTime(out var buildDateTime))
                    File.SetLastWriteTime(FullPath, buildDateTime);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Semaphore.Release();
            }

            resource.DisposableStream.Dispose();
        }

        public override bool Equals(Item? other)
        {
            if (other is not ResourceItem)
                return false;

            return base.Equals(other);
        }

        public override int GetHashCode()
            => HashCode.Combine(ResourceId, base.GetHashCode());

    }

    /// <summary>
    /// Tage referencing a remotely hosted file
    /// </summary>
    public class UriItem : AsynchronousSourcedItem
    {
        public Uri SourceUri { get; }

        public Uri? RootUri { get; }

        public string LocalPath { get; }

        /// <summary>
        /// Convert a Uri into an item key (replacing rootUri portion with a Guid to prevent naming conflicts)
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="rootUri">part of Uri replaced with Guid (default: "scheme://host")</param>
        /// <param name="folderPath"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static UriItem Get(Uri sourceUri, Uri? rootUri = null, string? folderPath = null,
            Assembly? assembly = null)
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

        private UriItem(Uri sourceUri, Uri? rootUri, string localPath, string fullPath, string? folderPath,
            Assembly assembly) : base(fullPath,
            folderPath, assembly)
        {
            SourceUri = sourceUri;
            RootUri = rootUri;
            LocalPath = localPath;
        }

        private static string RootFolderPath(string root, string? folderPath, Assembly assembly)
        {
            if (!ItemUriRootLookup.TryGetValue(root, out var itemGuid) || string.IsNullOrWhiteSpace(itemGuid))
            {
                itemGuid = Guid.NewGuid().ToString();
                ItemUriRootLookup[root] = itemGuid;
            }

            return Path.Combine(FullPathForFolderAndAssembly(folderPath, assembly), itemGuid);
        }

        /// <summary>
        /// Human-readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"SourceUri: {SourceUri}; RootUri: {RootUri}; {base.ToString()}";

        /// <summary>
        /// Is item available from source?
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> IsSourceAvailableAsync()
        {
            try
            {
                // Get the version / time-stamp headers
                using var headerRequest = new HttpRequestMessage(HttpMethod.Head, SourceUri);
                using var headerResponse = await HttpClient.SendAsync(headerRequest);

                return headerResponse.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                QLog.Error($"UriItemKey download exception: {ex.Message}");
                return false;
            }
        }
        
        public override async Task<bool> ForcePullAsync()
        {
            EntityTagHeaderValue? currentETag = null;
            try
            {
                // Get the version / time-stamp headers
                using var headerRequest = new HttpRequestMessage(HttpMethod.Head, SourceUri);
                using var headerResponse = await HttpClient.SendAsync(headerRequest);

                if (!headerResponse.IsSuccessStatusCode)
                    return false;

                currentETag = headerResponse.Headers.ETag;


                if (!headerResponse.IsSuccessStatusCode ||
                    headerResponse.Headers.TryGetValues("Last-Modified", out var headers) || 
                    headers?.FirstOrDefault() is not string lastModifiedString ||
                    !DateTime.TryParseExact(lastModifiedString, "R", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out var lastModified))
                    lastModified = DateTime.MinValue;

                if (Directory.Exists(FullPath))
                {
                    // Can't get file from server
                    Console.WriteLine($"Status Code: {headerResponse.StatusCode}");
                    if (!headerResponse.IsSuccessStatusCode)
                        return true;

                    // Current ETag matches stored ETag
                    if (ETagLookup.TryGetValue(FullPath, out var localETag) &&
                        currentETag?.Tag == localETag)
                        return true;

                    // Current server date-time matches local date-time 
                    var localWrite = Directory.GetLastWriteTime(FullPath);
                    if (lastModified != DateTime.MinValue && localWrite >= lastModified)
                        return true;
                }

                // Pull file from server
                var bytes = await HttpClient.GetByteArrayAsync(SourceUri);

                try
                {
                    await Semaphore.WaitAsync();
                    AssureParentDirectory();
                    await File.WriteAllBytesAsync(FullPath, bytes);

                    if (lastModified != DateTime.MinValue)
                        File.SetLastWriteTime(FullPath, lastModified);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Semaphore.Release();
                }

                if (currentETag?.Tag is null)
                    ETagLookup.Remove(FullPath);
                else
                    ETagLookup[FullPath] = currentETag.Tag;

                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public override bool Equals(Item? other)
        {
            if (other is not UriItem)
                return false;
            return base.Equals(other);
        }

        public override int GetHashCode()
            => HashCode.Combine(SourceUri, RootUri, base.GetHashCode());


    }


    #endregion


    #region Clear

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
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
    public static bool Clear(DateTime dateTime, Item? key = null)
    {
        key ??= TagItem.InternalGet(string.Empty, string.Empty, null);

        if (string.IsNullOrWhiteSpace(key.FullPath))
            return false;
        
        if (!key.FullPath.StartsWith(StorePath))
            throw new Exception($"Key [{key.FullPath}] does not start with P42.Utils.LocalData StorePath");

        try
        {
            Semaphore.Wait();
            if (File.Exists(key.FullPath))
            {
                File.Delete(key.FullPath);
                return true;
            }
            else if (Directory.Exists(key.FullPath))
            {
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


    
}



