using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoldersApp.Repositories
{
    public interface IFoldersRepository
    {
        Task<FileSystemItem> GetFileSystemItemByPath(string path);

        Task<List<FileSystemItem>> GetFolder(string path);

        Task<FileSystemItem> CreateFile(int? parentId, string name, string fullPath, string content, string fileType);

        Task<FileSystemItem> CreateFolder(int? parentId, string name, string fullPath);

        Task<FileSystemItem> ChangeFolder(FileSystemItem fileSystemItem);

        Task RemoveFolder(FileSystemItem fileSystemItem);
    }
}