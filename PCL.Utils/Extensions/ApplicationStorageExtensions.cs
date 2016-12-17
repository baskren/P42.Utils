using System.Reflection;
using System.IO;
using PCLStorage;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace PCL.Utils
{
	public static class ApplicationStorageExtensions
	{
		public static string EmbeddedStoredText(string resourceName, Assembly assembly = null)
		{
			string contents=null;
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream != null)
				using (var reader = new StreamReader(stream))
				{
					contents = reader.ReadToEnd();
				}
			}
			return contents;
		}

		public static string JsonFromStoredFolder(string fileName, IFolder folder)
		{
			string contents = null;
			if (folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
			{
				var file = folder.GetFile(fileName);
				contents = file?.ReadAllText();
			}
			return contents;
		}

		public static string LoadLocalStorageText(string fileName)
		{
			var folder = FileSystem.Current.LocalStorage;
			//System.Diagnostics.Debug.WriteLine("LocalFolder=" + folder?.Path);
			return folder == null ? null : JsonFromStoredFolder(fileName, folder);
		}

		public static string LoadRoamingStorageText(string fileName)
		{
			var folder = FileSystem.Current.RoamingStorage;
			//System.Diagnostics.Debug.WriteLine("RoamingFolder="+folder?.Path);
			return folder == null ? null : JsonFromStoredFolder(fileName, folder);
		}

		public static string LoadText(string fileName, Assembly assembly = null)
		{
			var result = LoadRoamingStorageText(fileName);
			result = result ?? LoadLocalStorageText(fileName);
			if (result == null)
			{
				assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
				result = EmbeddedStoredText(fileName, assembly);
			}
			return result;
		}

		public static void StoreText(string text, string fileName)
		{
			var folder = FileSystem.Current.LocalStorage;
			if (folder == null)
				throw new InvalidDataContractException("Hey, there should be a LocalStorage folder!");
			IFile file = folder.CreateFile(fileName, CreationCollisionOption.ReplaceExisting);
			//file.WriteAllText(text);
			file.WriteAllTextAsync(text);
		}

		public static TType LoadSerializedResource<TType>(string resourceName, Assembly assembly = null, TType defaultValue = default(TType))
		{
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
			var text = LoadText(resourceName, assembly);
			if (!string.IsNullOrEmpty(text))
			{
				TType result = JsonConvert.DeserializeObject<TType>(text, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});
				return result;
			}
			return defaultValue;
		}

		public static TType LoadSerializedEmbeddedResource<TType>(string resourceName, Assembly assembly = null, TType defaultValue = default(TType))
		{
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
			var text = EmbeddedStoredText(resourceName, assembly);
			if (!string.IsNullOrEmpty(text))
			{
				TType result = JsonConvert.DeserializeObject<TType>(text, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});
				return result;
			}
			return defaultValue;
		}


		public static void StoreSerializedResource<TType>(TType obj, string resourceName)
		{
			if (resourceName != null)
			{
				var jsonSerializationSetings = new JsonSerializerSettings { 
					TypeNameHandling = TypeNameHandling.All, 
					Formatting = Formatting.Indented,
					//PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					//ReferenceLoopHandling = ReferenceLoopHandling.Ignore
					ReferenceLoopHandling = ReferenceLoopHandling.Serialize
				};
				string textToStore = JsonConvert.SerializeObject(obj, jsonSerializationSetings);
				if (string.IsNullOrEmpty(textToStore))
					throw new InvalidDataContractException("Could not serialize object [" + obj + "]");
				StoreText(textToStore,resourceName);
			}
		}

		public static StreamReader ResourceStreamReader(string resourceName, Assembly assembly = null)
		{
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
			var streamReader = RoamingStreamReader(resourceName);
			if (streamReader == null)
				streamReader = LocalStreamReader(resourceName);
			if (streamReader == null)
				streamReader = EmbeddedResourceStreamReader(resourceName, assembly);
			return streamReader;
		}

		public static StreamReader EmbeddedResourceStreamReader(string resourceName, Assembly assembly = null)
		{
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
			var stream = assembly.GetManifestResourceStream(resourceName);
			var streamReader = new StreamReader(stream);
			return streamReader;
		}

		public static StreamReader RoamingStreamReader(string fileName)
		{
			var folder = FileSystem.Current.RoamingStorage;
			return folder == null ? null : StreamReaderFromStoredFolder(fileName, folder);
		}

		public static StreamReader LocalStreamReader(string fileName)
		{
			var folder = FileSystem.Current.LocalStorage;
			return folder == null ? null : StreamReaderFromStoredFolder(fileName, folder);
		}

		public static StreamReader StreamReaderFromStoredFolder(string fileName, IFolder folder)
		{
			StreamReader streamReader = null;
			var path = folder.Path + fileName;
			if (folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
			{
				var file = folder.GetFile(fileName);
				var stream = file.Open(FileAccess.Read);
				streamReader = new StreamReader(stream);
			}	
			return streamReader;
		}


		public static StreamWriter ResourceStreamWriter(string resourceName)
		{
			var streamReader = RoamingStreamWriter(resourceName);
			if (streamReader == null)
				streamReader = LocalStreamWriter(resourceName);
			return streamReader;
		}

		public static StreamWriter RoamingStreamWriter(string fileName)
		{
			var folder = FileSystem.Current.RoamingStorage;
			return folder == null ? null : StreamWriterFromStoredFolder(fileName, folder);
		}

		public static StreamWriter LocalStreamWriter(string fileName)
		{
			var folder = FileSystem.Current.LocalStorage;
			return folder == null ? null : StreamWriterFromStoredFolder(fileName, folder);
		}

		public static StreamWriter StreamWriterFromStoredFolder(string fileName, IFolder folder)
		{
			StreamWriter streamReader = null;
			var path = folder.Path + fileName;
			if (folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
			{
				var file = folder.GetFile(fileName);
				var stream = file.Open(FileAccess.ReadAndWrite);
				streamReader = new StreamWriter(stream);
			}
			return streamReader;
		}


	}
}

