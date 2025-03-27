using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using P42.Serilog.QuickLog;

namespace P42.Utils;



/// <summary>
/// base for storage/recall of items in local data store for common caching scenarios
/// </summary>
public abstract class LocalData
{

    #region Format Implementations

    /// <summary>
    /// Directory handler for LocalData store
    /// </summary>
    public static LocalDataDirectoryInfo DirectoryInfo => LocalDataDirectoryInfo.Instance;

    /// <summary>
    /// File handler for LocalData store
    /// </summary>
    public static LocalDataFileInfo FileInfo => LocalDataFileInfo.Instance;

    /// <summary>
    /// TextHandler for LocalData store
    /// </summary>
    public static LocalDataText Text => LocalDataText.Instance;

    /// <summary>
    /// Stream handler for LocalData store
    /// </summary>
    public static LocalDataStream Stream => LocalDataStream.Instance;

    /// <summary>
    /// StreamReader handler for LocalData store
    /// </summary>
    public static LocalDataStreamReader StreamReader => LocalDataStreamReader.Instance;

    /// <summary>
    /// StreamWriter handler for LocalData store
    /// </summary>
    public static LocalDataStreamWriter StreamWriter => LocalDataStreamWriter.Instance;

    #endregion

    
    #region Fields

    public static readonly SemaphoreSlim Semaphore = new (1, 1);
    
    private static readonly JsonSerializerSettings JsonSerializationSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize
    };


    /// <summary>
    /// Used to set folder used for local data store.  
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
#pragma warning disable CS0618 // Type or member is obsolete
    // DO NOT CHANGE Environment.ApplicationDataPath to another path.
    // This is used to pass EmbeddedResource Fonts to UWP Text elements and there is zero flexibility here.
    private static string PlatformFolder => Environment.ApplicationLocalFolderPath;
#pragma warning restore CS0618 // Type or member is obsolete
    private const string StoreFolderName = "P42.Utils.LocalDataStore";
    private static readonly string StorePath = System.IO.Path.Combine(PlatformFolder, StoreFolderName);
    private static readonly string ItemUriRootLookupPath = System.IO.Path.Combine(StorePath, nameof(ItemUriRootLookup));
    private static readonly string ETagLookupPath = System.IO.Path.Combine(StorePath, nameof(ETagLookup));

    // ReSharper disable StaticMemberInGenericType
    private static readonly ObservableConcurrentDictionary<string, string> ItemUriRootLookup;
    protected static readonly ObservableConcurrentDictionary<string, string> ETagLookup;


    protected static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> ItemLoadingTasks = new();
    protected static readonly HttpClient HttpClient = new();
    // ReSharper restore StaticMemberInGenericType

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

        var versionFilePath = System.IO.Path.Combine(platformFolder.FullName, "P42.Utils.LocalData.version.txt");

        if (!DirectoryExtensions.TryGetOrCreateDirectory(StorePath, out var storeFolder))
            throw new Exception($"Cannot create StoreFolder [{StorePath}] forP42.Utils.LocalData");
        if (storeFolder is null)
            throw new Exception($"Cannot get StoreFolder [{StorePath}] for P42.Utils.LocalData");

        if (System.IO.File.Exists(versionFilePath))
        {
            var oldVersion = System.IO.File.ReadAllText(versionFilePath);
            if (!string.IsNullOrWhiteSpace(oldVersion) && oldVersion != version.ToString())
            {
                storeFolder.Delete(true);
                storeFolder.Create();
            }
        }

        System.IO.File.WriteAllText(versionFilePath, version.ToString());

        if (System.IO.File.Exists(ItemUriRootLookupPath) &&
            System.IO.File.ReadAllText(ItemUriRootLookupPath) is { } urJson &&
            !string.IsNullOrWhiteSpace(urJson) &&
            JsonConvert.DeserializeObject<ObservableConcurrentDictionary<string, string>>(urJson) is { } uriLookup)
            ItemUriRootLookup = uriLookup;
        else
            ItemUriRootLookup = new ObservableConcurrentDictionary<string, string>();
        
        if (System.IO.File.Exists(ETagLookupPath) &&
            System.IO.File.ReadAllText(ETagLookupPath) is { } etagJson &&
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
                await System.IO.File.WriteAllTextAsync(ETagLookupPath, json);
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
                await System.IO.File.WriteAllTextAsync(ItemUriRootLookupPath, json);
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
    public static List<string> List(ItemKey key)
    {
        if (System.IO.File.Exists(key.Path))
            return [key.Path];

        if (System.IO.Directory.Exists(key.Path))
            return System.IO.Directory.EnumerateFiles(key.Path, "*", System.IO.SearchOption.AllDirectories).ToList();
        
        return [];
    }

    #endregion


    #region ItemKey

    public abstract class ItemKey(string path, string? folderPath, Assembly assembly)
    {
        /// <summary>
        /// Used to compartmentalize items by assembly (default: current application assembly)
        /// </summary>
        public Assembly Assembly { get; } = assembly;

        /// <summary>
        /// FolderPath (used to further compartmentalize items (default: null)
        /// </summary>
        public string FolderPath { get; } = folderPath?.Trim('/').Trim('\\') ?? string.Empty;

        /// <summary>
        /// File system path to item
        /// </summary>
        public string Path { get; } = path;

        /// <summary>
        /// "ms-appdata://" path to item
        /// </summary>
        public string AppDataUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Path) || !Path.StartsWith(PlatformFolder))
                    return string.Empty;

                var localPathFragment = Path[PlatformFolder.Length..].Replace('\\', '/').Trim('/');
                return $"ms-appdata:///local/{localPathFragment}";

            }
        }
        
        

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
        /// <param name="folderPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected static string PathForFolderPathAndAssembly(string? folderPath, Assembly assembly)
        {
            folderPath = folderPath?.Trim('/').Trim('\\') ?? string.Empty;
            return System.IO.Path.Combine(StoreFolderName, assembly.Name(), folderPath);
        }

        /// <summary>
        /// Human-readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"FolderPath: {FolderPath}; Assembly: {Assembly.Name()}";

        /// <summary>
        /// Is the item stored locally?
        /// </summary>
        public bool Exists => System.IO.File.Exists(Path) || System.IO.Directory.Exists(Path);
        
        /// <summary>
        /// Is the item stored as a directory?
        /// </summary>
        public bool IsDirectory => System.IO.Directory.Exists(Path);
        
        /// <summary>
        /// Is the item stored as a file?
        /// </summary>
        public bool IsFile => System.IO.File.Exists(Path);
        
        /// <summary>
        /// Is the item available from the source?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<bool> IsSourceAvailableAsync() => throw new NotImplementedException();


        public virtual Task<bool> TryRecallOrPullItemAsync() => throw new NotImplementedException();

    }

    public class TagItemKey : ItemKey
    {
        public string Tag { get; }

        public static TagItemKey Get(string tag, string? folderPath = null, Assembly? assembly = null)
        {
            tag = CleanKey(tag);
            var givenAssembly = assembly;
            assembly =  EmbeddedResourceExtensions.FindAssembly(tag, givenAssembly);
            if (assembly is null)
                throw new ArgumentException( $"Cannot find resourceId [{tag}] in provided assembly [{givenAssembly?.Name() ?? "null"}]");
            
            var rootPath = System.IO.Path.Combine(PathForFolderPathAndAssembly(folderPath, assembly), tag);
            if (!DirectoryExtensions.TryGetOrCreateDirectory(rootPath, out var rootDirectory))
                throw new Exception($"Cannot create folder [{rootPath}] for P42.Utils.LocalData");
            
            var path = rootDirectory.FullName ?? throw new Exception($"Cannot get rootDirectory.FullName for rootPath [{rootPath}] in P42.Utils.LocalData");
            if (path.Length >= 260)
                throw new Exception($"Path [{path}] too long [{path.Length}] for P42.Utils.LocalData");

            return new TagItemKey(path, tag, folderPath, assembly);

        }

        private TagItemKey(string path, string tag, string? folderPath, Assembly assembly) : base(path, folderPath, assembly)
        {
            Tag = tag;
        }
        
        public override string ToString()
            => $"Tag: {Tag}; FolderPath: {FolderPath}; {base.ToString()}";
        
        /// <summary>
        /// Is the item available from the source?
        /// </summary>
        /// <returns></returns>
        public override Task<bool> IsSourceAvailableAsync() => Task.FromResult(false);
        
        
        public override Task<bool> TryRecallOrPullItemAsync()
            => Task.FromResult(System.IO.Directory.Exists(Path) || System.IO.File.Exists(Path));
        
    }
    
    public class ResourceItemKey : ItemKey
    {
        public string ResourceId { get; }
        
        public static ResourceItemKey Get(string resourceId, string? folderPath = null, Assembly? assembly = null) 
        {
            resourceId = CleanKey(resourceId);
            var givenAssembly = assembly;
            assembly =  EmbeddedResourceExtensions.FindAssembly(resourceId, givenAssembly);
            if (assembly is null)
                throw new ArgumentException( $"Cannot find resourceId [{resourceId}] in provided assembly [{givenAssembly?.Name() ?? "null"}]");
            
            var rootPath = System.IO.Path.Combine(PathForFolderPathAndAssembly(folderPath, assembly), resourceId);
            if (!DirectoryExtensions.TryGetOrCreateDirectory(rootPath, out var rootDirectory))
                throw new Exception($"Cannot create folder [{rootPath}] for P42.Utils.LocalData");
            
            var path = rootDirectory.FullName ?? throw new Exception($"Cannot get rootDirectory.FullName for rootPath [{rootPath}] in P42.Utils.LocalData");
            if (path.Length >= 260)
                throw new Exception($"Path [{path}] too long [{path.Length}] for P42.Utils.LocalData");

            return new ResourceItemKey(path, resourceId, folderPath, assembly);
        }

        private ResourceItemKey(string path, string resourceId, string? folderPath, Assembly assembly) : 
            base(path, folderPath, assembly)
        {
            ResourceId = resourceId;
        }

        /// <summary>
        /// Human-readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"Resource: {ResourceId}; FolderPath: {FolderPath}; {base.ToString()}";
        
        /// <summary>
        /// Is the item available from the source?
        /// </summary>
        /// <returns></returns>
        public override Task<bool> IsSourceAvailableAsync() => Task.FromResult(Assembly.Exists(ResourceId));
        
        
        public override async Task<bool> TryRecallOrPullItemAsync()
        {
            if (System.IO.Directory.Exists(Path) || System.IO.File.Exists(Path))
                return true;
        
            if (EmbeddedResourceExtensions.FindAssemblyAndStream(ResourceId, Assembly) is not {} resource)
                return false;

            try
            {
                await using var destinationStream = new System.IO.FileStream(Path, System.IO.FileMode.Create);
                await Semaphore.WaitAsync();
                await resource.DisposableStream.CopyToAsync(destinationStream);

                var buildDateTime = Assembly.GetBuildTime();
                System.IO.File.SetLastWriteTime(Path, buildDateTime);
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

            await resource.DisposableStream.DisposeAsync();
            return true;
        }
        
        public virtual bool TryRecallOrPullItem()
        {
            if (System.IO.Directory.Exists(Path) || System.IO.File.Exists(Path))
                return true;
        
            if (EmbeddedResourceExtensions.FindAssemblyAndStream(ResourceId, Assembly) is not {} resource)
                return false;

            try
            {
                using var destinationStream = new System.IO.FileStream(Path, System.IO.FileMode.Create);
                Semaphore.Wait();
                resource.DisposableStream.CopyTo(destinationStream);

                var buildDateTime = Assembly.GetBuildTime();
                System.IO.File.SetLastWriteTime(Path, buildDateTime);
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

            resource.DisposableStream.Dispose();
            return true;
        }

    }

    public class UriItemKey : ItemKey
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
        public static UriItemKey Get(Uri sourceUri, Uri? rootUri = null, string? folderPath = null,
            Assembly? assembly = null)
        {
            assembly ??= AssemblyExtensions.GetApplicationAssembly();

            var root = HttpUtility.UrlDecode(sourceUri.Host);
            var localPath = HttpUtility.UrlDecode(sourceUri.LocalPath);

            if (rootUri is not null)
            {
                if (!sourceUri.AbsoluteUri.StartsWith(rootUri.AbsoluteUri))
                    throw new Exception(
                        $"SourceUri [{sourceUri}] does not start with RootUri [{rootUri}]for P42.Utils.LocalData");
                root = HttpUtility.UrlDecode(rootUri.AbsoluteUri);
                var relativeUri = sourceUri.AbsoluteUri[rootUri.AbsoluteUri.Length..];
                localPath = HttpUtility.UrlDecode(relativeUri);
            }

            var rootPath = UriRootFolderPath(root, folderPath, assembly);
            var path = System.IO.Path.Combine(rootPath, localPath);
            if (path.Length >= 260)
                throw new Exception($"Path [{path}] too long [{path.Length}] for P42.Utils.LocalData");

            if (!DirectoryExtensions.TryGetOrCreateDirectory(path, out var itemParent, true))
                throw new Exception($"Cannot create folder [{rootPath}] for P42.Utils.LocalData");

            if (itemParent.FullName is null)
                throw new Exception($"Cannot get itemParent.FullName for [{path}] in P42.Utils.LocalData");

            return new UriItemKey(path, localPath, sourceUri, rootUri, folderPath, assembly);
        }

        private UriItemKey(string path, string localPath, Uri sourceUri, Uri? rootUri, string? folderPath,
            Assembly assembly) : base(path,
            folderPath, assembly)
        {
            SourceUri = sourceUri;
            RootUri = rootUri;
            LocalPath = localPath;
        }

        private static string UriRootFolderPath(string root, string? folderPath, Assembly assembly)
        {
            if (!ItemUriRootLookup.TryGetValue(root, out var itemGuid) || string.IsNullOrWhiteSpace(itemGuid))
            {
                itemGuid = Guid.NewGuid().ToString();
                ItemUriRootLookup[root] = itemGuid;
            }

            return System.IO.Path.Combine(PathForFolderPathAndAssembly(folderPath, assembly), itemGuid);
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
        
        public override async Task<bool> TryRecallOrPullItemAsync()
        {
            try
            {
                // Get the version / time-stamp headers
                using var headerRequest = new HttpRequestMessage(HttpMethod.Head, SourceUri);
                using var headerResponse = await HttpClient.SendAsync(headerRequest);

                if (!headerResponse.IsSuccessStatusCode ||
                    headerResponse.Headers.GetValues("ETag").FirstOrDefault() is not { } currentETag)
                    currentETag = null;

                if (!headerResponse.IsSuccessStatusCode ||
                    headerResponse.Headers.GetValues("Last-Modified").FirstOrDefault() is not { } lastModifiedString ||
                    !DateTime.TryParseExact(lastModifiedString, "R", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out var lastModified))
                    lastModified = DateTime.MinValue;

                if (System.IO.Directory.Exists(Path))
                {
                    // Can't get file from server
                    Console.WriteLine($"Status Code: {headerResponse.StatusCode}");
                    if (!headerResponse.IsSuccessStatusCode)
                        return true;

                    // Current ETag matches stored ETag
                    if (!string.IsNullOrWhiteSpace(currentETag) &&
                        ETagLookup.TryGetValue(Path, out var localETag) &&
                        currentETag == localETag)
                        return true;

                    // Current server date-time matches local date-time 
                    var localWrite = System.IO.Directory.GetLastWriteTime(Path);
                    if (lastModified != DateTime.MinValue && localWrite >= lastModified)
                        return true;
                }

                // Pull file from server
                var bytes = await HttpClient.GetByteArrayAsync(SourceUri);

                try
                {
                    await Semaphore.WaitAsync();
                    await System.IO.File.WriteAllBytesAsync(Path, bytes);

                    if (lastModified != DateTime.MinValue)
                        System.IO.File.SetLastWriteTime(Path, lastModified);
                }
                catch (Exception e)
                {
                    QLog.Error(e, "UriItemKey write exception");
                    return System.IO.Directory.Exists(Path);
                }
                finally
                {
                    Semaphore.Release();
                }

                if (string.IsNullOrEmpty(currentETag))
                    ETagLookup.Remove(Path);
                else
                    ETagLookup[Path] = currentETag;

                return true;
            }
            catch (Exception ex)
            {
                QLog.Error(ex, "UriItemKey download exception");
                return System.IO.Directory.Exists(Path);
            }

        }

    }

    #endregion
    

    #region Clear

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
    public static bool Clear(ItemKey key)
        => Clear(DateTime.Now, key);

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="timeSpan">only items equal to or older than</param>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
    public static bool Clear(TimeSpan timeSpan, ItemKey key)
        => Clear(DateTime.Now - timeSpan, key);

    /// <summary>
    /// Clear item(s) from local data store.
    /// </summary>
    /// <param name="dateTime">only items equal to or older than</param>
    /// <param name="key">omit to clear all</param>
    /// <returns>true if any items cleared</returns>
    public static bool Clear(DateTime dateTime, ItemKey key)
    {
        if (string.IsNullOrWhiteSpace(key.Path))
            return false;
        
        if (!key.Path.StartsWith(StorePath))
            throw new Exception($"Key [{key.Path}] does not start with P42.Utils.LocalData StorePath");
        
        var parts = key.Path.Split(['/','\\'], StringSplitOptions.RemoveEmptyEntries);
        var parentPath = key.Path[..^parts.Last().Length];
        var parentFolder = new System.IO.DirectoryInfo(parentPath);
        if (!parentFolder.Exists)
            return false;

        var filesDeleted = false;
        try
        {
            var candidates = parentFolder
                .GetFileSystemInfos(parts.Last())
                .Where(c=> c.Exists && c.LastWriteTime <= dateTime)
                .ToArray();
            Semaphore.Wait();
            foreach (var child in candidates)
            {
                child.Delete();
                filesDeleted = true;
            }
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

        return filesDeleted;
    }

    #endregion
    

    #region Serialize / Deserialize
    
    /// <summary>
    /// Load serialized object from app local storage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? Deserialize<T>(ItemKey key, T? defaultValue = default)
    {
        if (!Text.TryRecallItem(out var text, key) || string.IsNullOrWhiteSpace(text))
            return defaultValue;

        return JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }

    /// <summary>
    /// Load serialized object from app local storage
    /// </summary>
    /// <param name="key"></param>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>false on fail</returns>
    public static bool TryDeserialize<T>(ItemKey key, out T? result)
    {
        result = default;
        if (!Text.TryRecallItem(out var text, key) || string.IsNullOrWhiteSpace(text))
            return false;

        try
        {
            result = JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return true;
        }
        catch (Exception e)
        {
            QLog.Error(e);
            return false;
        }
    }

    /// <summary>
    /// Deserializes item from local data store unless not there or ItemKey.Source is more up to date.  Then, first pulls item from ItemKey source before deserializing.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> RecallOrPullAndDeserializeAsync<T>(ItemKey key, T? defaultValue = default)
    {
        return await Text.RecallOrPullItemAsync(key) is {} 
            ? Deserialize(key, defaultValue)
            : defaultValue;
    }

    /// <summary>
    /// Attempt to serialize object to app local storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="key"></param>
    /// <returns>true on success</returns>
    public static void Serialize<T>(T? obj, ItemKey key)
    {
        var json = JsonConvert.SerializeObject(obj, JsonSerializationSettings);
        Text.StoreItem(json, key);
    }

    /// <summary>
    /// Attempt to serialize object to app local storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="key"></param>
    /// <returns>true on success</returns>
    public static async Task<bool> TrySerializeAsync<T>(T obj, ItemKey key)
    {
        try
        {
            var json = JsonConvert.SerializeObject(obj, JsonSerializationSettings);
            return await Text.TryStoreItemAsync(json, key);
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
    }
    
    #endregion
    
}

public abstract class LocalData<T> : LocalData
{

    #region Recall Item

    /// <summary>
    /// Try to recall item from local data store.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>null if not already in local data store</returns>
    public abstract bool TryRecallItem(out T? item, ItemKey key);

    /// <summary>
    /// Recalls item from local data store unless not there or ItemKey.Source is more up to date.  Then pulls item from ItemKey source.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null if item not available</returns>
    public abstract Task<T?> RecallOrPullItemAsync(ItemKey key);
    
    #endregion


    #region Store

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="System.IO.IOException"></exception>
    public abstract void StoreItem(T? sourceItem, ItemKey key, bool wipeOld = true);
    
    /// <summary>
    /// Store item in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    public abstract Task StoreItemAsync(T? sourceItem, ItemKey key, bool wipeOld = true);

    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="sourceItem">null to clear</param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public virtual bool TryStoreItem(T? sourceItem, ItemKey key, bool wipeOld = true)
    {
        try
        {
            StoreItem(sourceItem, key, wipeOld);
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
    }
    
    
    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="sourceItem">null to clear</param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public virtual async Task<bool> TryStoreItemAsync(T? sourceItem, ItemKey key, bool wipeOld = true)
    {
        try
        {
            await StoreItemAsync(sourceItem, key, wipeOld);
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        
        return false;
    }
    
    #endregion
    
}
