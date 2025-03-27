using System;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using P42.Serilog.QuickLog;


namespace P42.Utils;

// ReSharper disable once UnusedType.Global
[Obsolete("Use P42.Utils.LocalData, instead.")]
public static class AppLocalStorage
{
    
    #region OBSOLETE
    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead.", true)]
    public static string LocalPath(string assemblyName, string folder, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text, instead.", true)]
    public static string EmbeddedStoredText(string resourceId, Assembly? assembly = null)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text, instead.", true)]
    public static string JsonFromStorage(string fileName, string folder = "")
        => throw new NotImplementedException();
    
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text, instead.", true)]
    public static string LoadText(string uid, string fileName, Assembly? assembly = null)
        => throw new NotImplementedException();

    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem(), or .TryStoreItemAsync() in P42.Utils.LocalData.Text, instead.", true)]
    public static void StoreText(string uid, string text, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .Deserialize<T>() or .RecallOrPullAndDeserialize<T>() in P42.Utils.LocalData, instead.", true)]
    public static TType? LoadSerializedResource<TType>(string uid, string resourceName, Assembly? assembly = null, TType? defaultValue = default)
        => throw new NotImplementedException();

    [Obsolete("Use .Deserialize<T>() or .RecallOrPullAndDeserialize<T>() in P42.Utils.LocalData, instead.", true)]
    public static TType? LoadSerializedEmbeddedResource<TType>(string resourceName, Assembly? assembly = null, TType? defaultValue = default)
        => throw new NotImplementedException();

    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead.", true)]
    public static void StoreSerializedResource<TType>(string uid, TType obj, string resourceName)
        => throw new NotImplementedException();

    [Obsolete("Use EmbeddedResourceExtensions.Exists() or P42.Utils.LocalData.ItemKey.Exists(), instead.", true)]
    public static bool ResourceAvailable(string uid, string resourceName, Assembly? assembly = null, StreamSource source = StreamSource.Default)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader, instead.", true)]
    public static StreamReader? ResourceStreamReader(string uid, string resourceName, Assembly? assembly = null, StreamSource source = StreamSource.Default)
        => throw new NotImplementedException();

    [Obsolete("Use EmbeddedResourceExtensions.Exists() or P42.Utils.LocalData.ItemKey.Exists(), instead.", true)]
    public static bool EmbeddedResourceAvailable(string resourceId, Assembly? assembly = null)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? EmbeddedResourceStreamReader(string resourceId, Assembly? assembly = null)
        => throw new NotImplementedException();

    [Obsolete("Use EmbeddedResourceExtensions.Exists() or P42.Utils.LocalData.ItemKey.Exists(), instead.", true)]
    public static bool LocalResourceAvailable(string uid, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? LocalStreamReader(string uid, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter ResourceStreamWriter(string uid, string resourceName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter LocalStreamWriter(string uid, string fileName) 
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.List, instead", true)]
    public static string[] ListResources(StreamSource source, Assembly? assembly = null)
        => throw new NotImplementedException();
    #endregion


    #region TryLoadTextFromLocalStorage

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TryLoadTextFromLocalStorage(string fileName, out string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TryLoadTextFromLocalStorage(string folder, string fileName, out string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TryLoadTextFromLocalStorage(string assemblyName, string folder, string fileName, out string text)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TryLoadTextFromLocalStorage(Assembly assembly, string fileName, out string text)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TryLoadTextFromLocalStorage(Assembly assembly, string folder, string fileName, out string text)
        => throw new NotImplementedException();
    
    #endregion

    
    #region LoadTextFromLocalStorage

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static string LoadTextFromLocalStorage(string fileName)
        => throw new NotImplementedException();
    

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static string LoadTextFromLocalStorage(string folder, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static string LoadTextFromLocalStorage(string assemblyName, string folder, string fileName)
        => throw new NotImplementedException();
    
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static string LoadTextFromLocalStorage(Assembly assembly, string fileName)
        => throw new NotImplementedException();
    
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static string LoadTextFromLocalStorage(Assembly assembly, string folder, string fileName)
        => throw new NotImplementedException();
    #endregion


    #region TrySaveTextToLocalStorage
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TrySaveTextToLocalStorage(string fileName, string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TrySaveTextToLocalStorage(string folder, string fileName, string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TrySaveTextToLocalStorage(string assemblyName, string folder, string fileName, string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TrySaveTextToLocalStorage(Assembly assembly, string fileName, string text)
        => throw new NotImplementedException();

    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static bool TrySaveTextToLocalStorage(Assembly assembly, string folder, string fileName, string text)
        => throw new NotImplementedException();
    #endregion

    
    #region SaveTextToLocalStorage
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static void SaveTextToLocalStorage(string fileName, string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static void SaveTextToLocalStorage(string folder, string fileName, string text)
        => throw new NotImplementedException();

    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static void SaveTextToLocalStorage(string assemblyName, string folder, string fileName, string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static void SaveTextToLocalStorage(Assembly assembly, string fileName, string text)
        => throw new NotImplementedException();
    
    [Obsolete("Use .StoreItem(), .StoreItemAsync(), .TryStoreItem() or .TryStoreItemAsync() in P42.Utils.LocalData.Text instead.", true)]
    public static void SaveTextToLocalStorage(Assembly assembly, string folder, string fileName, string text)
        => throw new NotImplementedException();
    #endregion


    #region TryDeserializeFromLocalStorage
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TryDeserializeFromLocalStorage<T>(string fileName, out T? result, T? defaultValue = default)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TryDeserializeFromLocalStorage<T>(string folder, string fileName, out T? result, T? defaultValue = default)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TryDeserializeFromLocalStorage<T>(string assemblyName, string folder, string fileName, 
        out T? result, T? defaultValue = default)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TryDeserializeFromLocalStorage<T>(Assembly assembly, string fileName, out T? result, T? defaultValue = default)
        => throw new NotImplementedException();

    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TryDeserializeFromLocalStorage<T>(Assembly assembly, string folder, string fileName, out T? result, T? defaultValue = default)
        => throw new NotImplementedException();

    #endregion
    
    
    #region Deserialize
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static T? DeserializeLocalStorage<T>(string fileName, T? defaultValue = default)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static T? DeserializeLocalStorage<T>(string folder, string fileName, T? defaultValue = default)
        => throw new NotImplementedException();

    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static T? DeserializeLocalStorage<T>(string assemblyName, string folder, string fileName,
        T? defaultValue = default)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static T? DeserializeLocalStorage<T>(Assembly assembly, string fileName, T? defaultValue = default)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Deserialize(), .TryDeserialize(), or .RecallOrPullAndDeserializeAsync() in P42.Utils.LocalData, instead", true)]
    public static T? DeserializeLocalStorage<T>(Assembly assembly, string folder, string fileName, T? defaultValue = default)
        => throw new NotImplementedException();
    
    #endregion

    
    #region TrySerializeToLocalStorage

    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TrySerializeToLocalStorage<T>(string fileName, T obj)
        => throw new NotImplementedException();

    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TrySerializeToLocalStorage<T>(string folder, string fileName, T obj)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TrySerializeToLocalStorage<T>(string assemblyName, string folder, string fileName, T obj)
        => throw new NotImplementedException();

    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TrySerializeToLocalStorage<T>(Assembly assembly, string fileName, T obj)
        => throw new NotImplementedException();

    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static bool TrySerializeToLocalStorage<T>(Assembly assembly, string folder, string fileName, T obj)
        => throw new NotImplementedException();

    #endregion
    
    
    #region Serialize

    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static void SerializeToLocalStorage<T>(string fileName, T obj)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static void SerializeToLocalStorage<T>(string folder, string fileName, T obj)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static void SerializeToLocalStorage<T>(string assemblyName, string folder, string fileName, T obj)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static void SerializeToLocalStorage<T>(Assembly assembly, string fileName, T obj)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Serialize() or .TrySerializeAsync() in P42.Utils.LocalData, instead", true)]
    public static void SerializeToLocalStorage<T>(Assembly assembly, string folder, string fileName, T obj)
        => throw new NotImplementedException();
    
    #endregion

    
    #region Exists

    [Obsolete("Use .Exists() in P42.Utils.LocalData.ItemKey, instead", true)]
    public static bool Exists(string fileName)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Exists() in P42.Utils.LocalData.ItemKey, instead", true)]
    public static bool Exists(string folder, string fileName)
        => throw new NotImplementedException();
    
    [Obsolete("Use .Exists() in P42.Utils.LocalData.ItemKey, instead", true)]
    public static bool Exists(string assemblyName, string folder, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .Exists() in P42.Utils.LocalData.ItemKey, instead", true)]
    public static bool Exists(Assembly assembly, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .Exists() in P42.Utils.LocalData.ItemKey, instead", true)]
    public static bool Exists(Assembly assembly, string folder, string fileName)
        => throw new NotImplementedException();
    #endregion
    
    
    #region StreamReader
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? StreamReader(string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? StreamReader(string folder, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? StreamReader(string assemblyName, string folder, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? StreamReader(Assembly assembly, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamReader instead.", true)]
    public static StreamReader? StreamReader(Assembly assembly, string folder, string fileName)
        => throw new NotImplementedException();


    #endregion


    #region StreamWriter
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter StreamWriter(string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter StreamWriter(string folder, string fileName)
        => throw new NotImplementedException();
    
    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter StreamWriter(string assemblyName, string folder, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter StreamWriter(Assembly assembly, string fileName)
        => throw new NotImplementedException();

    [Obsolete("Use .TryRecallItem() or .RecallOrPullItemAsync() in P42.Utils.LocalData.StreamWriter instead.", true)]
    public static StreamWriter StreamWriter(Assembly assembly, string folder, string fileName)
        => throw new NotImplementedException();
    #endregion
    
  
}
