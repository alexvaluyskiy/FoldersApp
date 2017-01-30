using FoldersApp.Core.Exceptions;
using FoldersApp.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoldersApp.Services
{
    public class FoldersService
    {
        private IFoldersRepository _foldersRepository;

        public FoldersService(IFoldersRepository foldersRepository)
        {
            _foldersRepository = foldersRepository;
        }

        public async Task<List<FolderTreeItem>> GetFolder(string path)
        {
            // Check the path if exists
            var pathItem = await _foldersRepository.GetFileSystemItemByPath(path);
            bool isPathExists = string.IsNullOrEmpty(path) ? true : pathItem != null;

            if (!isPathExists)
            {
                throw new FileNotFoundException(path);
            }

            // Check that a folder is not already exists
            var list = await _foldersRepository.GetFolder(path);

            List<FolderTreeItem> tree;

            if (list.Count == 1 && list[0].Type == FileSystemType.File)
            {
                list[0].ParentId = null;
                tree = GenerateTree(list, null);
            }
            else
            {
                tree = GenerateTree(list, pathItem?.Id);
            }

            if (tree == null)
                return new List<FolderTreeItem>();

            return tree;
        }

        public async Task<FileSystemItem> CreateFile(string path, string name, string content)
        {
            var fullPath = GenerateFullPath(path, name);

            // Check the path if exists
            var pathItem = await _foldersRepository.GetFileSystemItemByPath(path);
            bool isPathExists = string.IsNullOrEmpty(path) ? true : pathItem != null;

            // Check that a folder is not already exists
            var file = await _foldersRepository.GetFileSystemItemByPath(fullPath);
            bool fileAlreadyExists = file != null;

            if (fileAlreadyExists)
            {
                throw new FileAlreadyExistsException(name);
            }

            if (!isPathExists)
            {
                throw new FileNotFoundException(name);
            }

            string fileType = Path.GetExtension(name);

            return await _foldersRepository.CreateFile(pathItem?.Id, name, fullPath, content, fileType);
        }

        public async Task<FileSystemItem> CreateFolder(string path, string name)
        {
            var fullPath = GenerateFullPath(path, name);

            // Check the path if exists
            var pathItem = await _foldersRepository.GetFileSystemItemByPath(path);
            bool isPathExists = string.IsNullOrEmpty(path) ? true : pathItem != null;

            // Check that a folder is not already exists
            var folder = await _foldersRepository.GetFileSystemItemByPath(fullPath);
            bool folderAlreadyExists = folder != null;

            if (folderAlreadyExists)
            {
                throw new FileAlreadyExistsException(name);
            }

            if (!isPathExists)
            {
                throw new FileNotFoundException(name);
            }

             return await _foldersRepository.CreateFolder(pathItem?.Id, name, fullPath);
        }

        public async Task<FileSystemItem> Move(string currentFolderPath, string newPath)
        {
            // check that current folder is exists
            var currentFolderPathItem = await _foldersRepository.GetFileSystemItemByPath(currentFolderPath);
            if (currentFolderPathItem == null)
            {
                throw new FileNotFoundException(currentFolderPath);
            }

            // check that the root folder of new path is exists
            var newRootPath = GetRootPath(newPath);
            var newRootPathItem = await _foldersRepository.GetFileSystemItemByPath(newRootPath);
            if (newRootPathItem == null)
            {
                throw new FileNotFoundException(newRootPath);
            }

            currentFolderPathItem.ParentId = newRootPathItem.Id;
            currentFolderPathItem.FullPath = newPath;

            return await _foldersRepository.ChangeFolder(currentFolderPathItem);
        }

        public async Task Remove(string fullPath)
        {
            var pathItem = await _foldersRepository.GetFileSystemItemByPath(fullPath);

            if (pathItem == null)
            {
                throw new FileNotFoundException(fullPath);
            }

            await _foldersRepository.RemoveFolder(pathItem);
        }

        private List<FolderTreeItem> GenerateTree(List<FileSystemItem> list, int? parentId)
        {
            var tempList = list.Where(x => x.ParentId == parentId).Select(x => new FolderTreeItem
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Children = GenerateTree(list, x.Id)
            }).ToList();

            return tempList.Count > 0 ? tempList : null;
        }

        private string GenerateFullPath(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
                return name;

            return string.Concat(path, "/", name);
        }

        private string GetRootPath(string originalPath)
        {
            var pathSegments = originalPath.Split('/');
            Array.Resize(ref pathSegments, pathSegments.Length - 1);
            return string.Join("/", pathSegments);
        }
    }
}
