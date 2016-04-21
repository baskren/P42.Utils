using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;

namespace PCL.Utils
{
	public static class PCLStorageExtensions 
	{



		#region IFolder extensions
		public static ExistenceCheckResult CheckExists(this IFolder folder, string name) {
			if (string.IsNullOrEmpty (name))
				return ExistenceCheckResult.NotFound;
			Task<ExistenceCheckResult> task = Task.Run (() => folder.CheckExistsAsync (name));
			return task.Result;
		}

		public static IFile CreateFile (this IFolder folder, string desiredName, CreationCollisionOption option) {
			Task<IFile> task = Task.Run (() => folder.CreateFileAsync (desiredName, option));
			return task.Result;
		}

		public static IFolder CreateFolder (this IFolder folder, string desiredName, CreationCollisionOption option) {
			Task<IFolder> task = Task.Run (() => folder.CreateFolderAsync (desiredName, option));
			return task.Result;
		}

		public static void Delete(this IFolder folder) {
			Task.Run (() => folder.DeleteAsync ());
			return;
		}

		public static IFile GetFile (this IFolder folder, string name) {
			if (folder.CheckExists (name) == ExistenceCheckResult.NotFound)
				return null;
			Task<IFile> task = Task.Run (() => folder.GetFileAsync (name));
			return task.Result;
		}

		public static IList<IFile> GetFiles (this IFolder folder) {
			Task<IList<IFile>> task = Task.Run (() => folder.GetFilesAsync ());
			return task.Result;
		}

		public static IFolder GetFolder (this IFolder folder, string name) {
			if (folder.CheckExists (name) == ExistenceCheckResult.NotFound)
				return null;
			Task<IFolder> task = Task.Run (() => folder.GetFolderAsync (name));
			return task.Result;
		}

		public static IList<IFolder> GetFolders (this IFolder folder) {
			Task<IList<IFolder>> task = Task.Run (() => folder.GetFoldersAsync ());
			return task.Result;
		}
		#endregion

		#region IFileSystem extensions
		public static IFile GetFileFromPath (this IFileSystem system, string path) {
			Task<IFile> task = Task.Run(() => system.GetFileFromPathAsync(path));
			return task.Result;
		}

		public static IFolder GetFolderFromPath (this IFileSystem system, string path) {
			Task<IFolder> task = Task.Run(() => system.GetFolderFromPathAsync(path));
			return task.Result;
		}
		#endregion

		#region IFile extensions
		public static void Delete (this IFile file) {
			Task.Run (() => file.DeleteAsync ());
			return;
		}

		public static void Move (this IFile file, string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) {
			Task.Run (() => file.MoveAsync (newPath, collisionOption));
			return;
		}

		public static Stream Open(this IFile file, FileAccess fileAccess) {
			Task<Stream> task = Task.Run (() => file.OpenAsync (fileAccess));
			return task.Result;
		}

		public static void Rename(this IFile file, string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) {
			Task.Run (() => file.RenameAsync (newName, collisionOption));
			return;
		}
		#endregion

		#region File extensions extensions
		public static string ReadAllText (this IFile file) {
			Task<string> task = Task.Run (() => file.ReadAllTextAsync ());
			return task.Result;
		}

		public static void WriteAllText (this IFile file, string contents) {
			Task.Run (() => file.WriteAllTextAsync (contents));
			return;
		}
		#endregion
	}
}

