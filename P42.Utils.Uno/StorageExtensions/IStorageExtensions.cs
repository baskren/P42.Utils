namespace P42.Utils.Uno;

public static class IStorageExtensions
{
    /// <param name="folder"></param>
    extension(IStorageFolder folder)
    {
        /// <summary>
        /// Human-readable folder tree
        /// </summary>
        /// <returns></returns>
        public string FolderTree()
            => DirectoryExtensions.FolderTree(folder.Path);

        /// <summary>
        /// Clear the contents of an IStorageFolder
        /// </summary>
        /// <returns></returns>
        public async Task DeleteChildrenAsync()
        {
            var items = await folder.GetItemsAsync();
            foreach (var child in items)
                await child.DeleteAsync();
        }
    }
}
