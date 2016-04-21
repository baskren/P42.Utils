using System;
using System.Reflection;
using System.IO;
using PCLStorage;
using Newtonsoft.Json;

namespace PCL.Utils
{
	public static class ApplicationStorageExtensions
	{
		public static string EmbeddedStoredText(string resourceName, Assembly assembly=null) {
			string contents;
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo ().GetDeclaredMethod ("GetCallingAssembly").Invoke (null, new object[0]);
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (var reader = new StreamReader(stream)) {
				contents = reader.ReadToEnd();
			}
			return contents;
		}

		public static string JsonFromStoredFolder(string fileName, IFolder folder) {
			string contents=null;
			if (folder.CheckExists (fileName) == ExistenceCheckResult.FileExists) {
				var file = folder.GetFile (fileName);
				contents = file?.ReadAllText ();
			}
			return contents;
		}

		public static string LocalStoredText(string fileName) {
			var folder = FileSystem.Current.LocalStorage;
			return folder == null ? null : JsonFromStoredFolder (fileName, folder);
		}

		public static string RoamingStoredText(string fileName) {
			var folder = FileSystem.Current.RoamingStorage;
			return folder == null ? null : JsonFromStoredFolder (fileName, folder);
		}

		public static string StoredText(string fileName, Assembly assembly=null) {
			var result = RoamingStoredText (fileName);
			result = result ?? LocalStoredText (fileName);
			if (result == null) {
				assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo ().GetDeclaredMethod ("GetCallingAssembly").Invoke (null, new object[0]);
				result = EmbeddedStoredText (fileName, assembly);
			}
			return result;
		}

		public static TType DeserializeStoredObject<TType>(string resourceName, Assembly assembly=null, TType defaultValue=default(TType))  {
			assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo ().GetDeclaredMethod ("GetCallingAssembly").Invoke (null, new object[0]);
			var text = StoredText (resourceName, assembly);
			if (!string.IsNullOrEmpty (text)) {
				TType result = JsonConvert.DeserializeObject<TType>(text);
				return result;
			}
			return defaultValue;
		}
	}
}

