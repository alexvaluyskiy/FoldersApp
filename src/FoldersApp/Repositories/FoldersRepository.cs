using FoldersApp.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoldersApp.Repositories
{
    public class FoldersRepository : IFoldersRepository
    {
        private FoldersContext _foldersContext;

        public FoldersRepository(FoldersContext foldersContext)
        {
            _foldersContext = foldersContext;
        }

        public async Task<FileSystemItem> GetFileSystemItemByPath(string path)
        {
            return await _foldersContext.FileSystem.Where(c => c.FullPath.Equals(path)).FirstOrDefaultAsync();
        }

        public async Task<List<FileSystemItem>> GetFolder(string path)
        {
            if (!string.IsNullOrEmpty(path))
                return await _foldersContext.FileSystem.Where(c => c.FullPath.StartsWith(path)).ToListAsync();
            else
                return await _foldersContext.FileSystem.ToListAsync();
        }

        public async Task<FileSystemItem> CreateFile(int? parentId, string name, string fullPath, string content, string fileType)
        {
            var file = new FileSystemItem();
            file.ParentId = parentId;
            file.Name = name;
            file.Type = FileSystemType.File;
            file.FullPath = fullPath;
            file.FileContent = content;
            file.FileType = fileType;

            _foldersContext.FileSystem.Add(file);
            await _foldersContext.SaveChangesAsync();

            return file;
        }

        public async Task<FileSystemItem> CreateFolder(int? parentId, string name, string fullPath)
        {
            var folder = new FileSystemItem();
            folder.ParentId = parentId;
            folder.Name = name;
            folder.Type = FileSystemType.Folder;
            folder.FullPath = fullPath;

            _foldersContext.FileSystem.Add(folder);
            await _foldersContext.SaveChangesAsync();

            return folder;
        }

        public async Task<FileSystemItem> ChangeFolder(FileSystemItem fileSystemItem)
        {
            _foldersContext.Update(fileSystemItem);
            await _foldersContext.SaveChangesAsync();

            return fileSystemItem;
        }

        public async Task RemoveFolder(FileSystemItem fileSystemItem)
        {
            _foldersContext.FileSystem.Remove(fileSystemItem);
            await _foldersContext.SaveChangesAsync();
        }
    }
}
