using FoldersApp.Core.Exceptions;
using FoldersApp.Repositories;
using FoldersApp.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FoldersApp.Tests
{
    public class FoldersServiceTests
    {
        [Fact]
        public async Task GetFolder_should_build_a_tree_of_folders_and_files_in_root_folder()
        {
            string path = "";
            var listOfFolders = new List<FileSystemItem>()
            {
                new FileSystemItem() { Id = 1, ParentId = null, Name = "animals", FullPath = "animals", Type = FileSystemType.Folder },
                new FileSystemItem() { Id = 2, ParentId = 1, Name = "dogs", FullPath = "animals/dogs", Type = FileSystemType.Folder },
                new FileSystemItem() { Id = 3, ParentId = 2, Name = "somedog.txt", FullPath = "animals/somedog.txt", Type = FileSystemType.File }
            };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(null);
            foldersRepositoryMock.Setup(c => c.GetFolder(It.IsAny<string>())).ReturnsAsync(listOfFolders);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.GetFolder(path);
            Assert.NotNull(actualFolder);
            Assert.Equal(1, actualFolder[0].Id);
            Assert.Equal("animals", actualFolder[0].Name);
            Assert.Equal(FileSystemType.Folder, actualFolder[0].Type);

            Assert.Equal(2, actualFolder[0].Children[0].Id);
            Assert.Equal("dogs", actualFolder[0].Children[0].Name);
            Assert.Equal(FileSystemType.Folder, actualFolder[0].Children[0].Type);

            Assert.Equal(3, actualFolder[0].Children[0].Children[0].Id);
            Assert.Equal("somedog.txt", actualFolder[0].Children[0].Children[0].Name);
            Assert.Equal(FileSystemType.File, actualFolder[0].Children[0].Children[0].Type);
        }

        [Fact]
        public async Task GetFolder_should_build_a_tree_of_folders_and_files_in_sub_folder()
        {
            string path = "animals";
            var foundPath = new FileSystemItem() { Id = 1, ParentId = null, Name = "animals", FullPath = "animals", Type = FileSystemType.Folder };
            var listOfFolders = new List<FileSystemItem>()
            {
                foundPath,
                new FileSystemItem() { Id = 2, ParentId = 1, Name = "dogs", FullPath = "animals/dogs", Type = FileSystemType.Folder },
                new FileSystemItem() { Id = 3, ParentId = 1, Name = "cats", FullPath = "animals/cats", Type = FileSystemType.Folder },
                new FileSystemItem() { Id = 4, ParentId = 2, Name = "somedog.txt", FullPath = "animals/somedog.txt", Type = FileSystemType.File },
            };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(foundPath);
            foldersRepositoryMock.Setup(c => c.GetFolder(It.IsAny<string>())).ReturnsAsync(listOfFolders);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.GetFolder(path);
            Assert.NotNull(actualFolder);

            Assert.Equal(2, actualFolder[0].Id);
            Assert.Equal("dogs", actualFolder[0].Name);
            Assert.Equal(FileSystemType.Folder, actualFolder[0].Type);

            Assert.Equal(3, actualFolder[1].Id);
            Assert.Equal("cats", actualFolder[1].Name);
            Assert.Equal(FileSystemType.Folder, actualFolder[1].Type);

            Assert.Equal(4, actualFolder[0].Children[0].Id);
            Assert.Equal("somedog.txt", actualFolder[0].Children[0].Name);
            Assert.Equal(FileSystemType.File, actualFolder[0].Children[0].Type);
        }

        [Fact]
        public async Task GetFolder_should_build_a_tree_of_empty_folder()
        {
            string path = "animals/dogs";
            var foundPath = new FileSystemItem() { Id = 1, ParentId = null, Name = "animals", FullPath = "animals", Type = FileSystemType.Folder };
            var listOfFolders = new List<FileSystemItem>()
            {
                foundPath,
                new FileSystemItem() { Id = 2, ParentId = 1, Name = "dogs", FullPath = "animals/dogs", Type = FileSystemType.Folder }
            };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(foundPath);
            foldersRepositoryMock.Setup(c => c.GetFolder(It.IsAny<string>())).ReturnsAsync(listOfFolders);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.GetFolder(path);
            Assert.NotNull(actualFolder);

            Assert.Equal(2, actualFolder[0].Id);
            Assert.Equal("dogs", actualFolder[0].Name);
            Assert.Equal(FileSystemType.Folder, actualFolder[0].Type);
        }

        [Fact]
        public async Task GetFolder_should_read_a_file()
        {
            string path = "animals/dogs/somedog.txt";
            var foundPath = new FileSystemItem()
            {
                Id = 3,
                ParentId = 2,
                Name = "somedog.txt",
                FullPath = "animals/dogs/somedog.txt",
                Type = FileSystemType.File,
                FileContent = "Content of file"
            };

            var listOfFolders = new List<FileSystemItem>()
            {
                foundPath
            };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(foundPath);
            foldersRepositoryMock.Setup(c => c.GetFolder(It.IsAny<string>())).ReturnsAsync(listOfFolders);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.GetFolder(path);
            Assert.NotNull(actualFolder);

            Assert.Equal(3, actualFolder[0].Id);
            Assert.Equal("somedog.txt", actualFolder[0].Name);
            Assert.Equal(FileSystemType.File, actualFolder[0].Type);
        }

        [Fact]
        public async Task GetFolder_should_throw_exception_if_path_is_not_found()
        {
            string path = "people/students";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(null);
            foldersRepositoryMock.Setup(c => c.GetFolder(It.IsAny<string>())).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileNotFoundException>(async () => await foldersService.GetFolder(path));
        }

        [Fact]
        public async Task CreateFile_should_create_file()
        {
            string name = "terrier.docx";
            string content = "content of the file";
            string fileType = ".docx";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.CreateFile(string.Empty, name, content);

            foldersRepositoryMock.Verify(c => c.CreateFile(
                It.Is<int?>(x => x.HasValue == false),
                It.Is<string>(x => x == name),
                It.Is<string>(x => x == name),
                It.Is<string>(x => x == content),
                It.Is<string>(x => x == fileType)));
        }

        [Fact]
        public async Task CreateFile_should_throw_exception_if_file_already_exists()
        {
            string path = "animals";
            string name = "dogs.txt";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(path)))).ReturnsAsync(null);
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(string.Concat(path, "/", name))))).ReturnsAsync(new FileSystemItem());

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileAlreadyExistsException>(async () => await foldersService.CreateFile("animals", name, string.Empty));
        }

        [Fact]
        public async Task CreateFolder_should_create_folder()
        {
            string name = "terrier";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.CreateFolder(string.Empty, name);

            foldersRepositoryMock.Verify(c => c.CreateFolder(It.Is<int?>(x => x.HasValue == false), It.Is<string>(x => x == name), It.Is<string>(x => x == name)));
        }

        [Fact]
        public async Task CreateFolder_should_create_subfolder()
        {
            string expectedName = "terrier";
            string expectedRootPath = "animals";
            string expectedFullPath = "animals/terrier";
            var expectedParentFolder = new FileSystemItem() { Id = 1, Name = expectedRootPath };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock
                .Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(expectedRootPath))))
                .ReturnsAsync(expectedParentFolder);

            foldersRepositoryMock
                .Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(expectedFullPath))))
                .ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.CreateFolder(expectedRootPath, expectedName);

            foldersRepositoryMock
                .Verify(c => c.CreateFolder(
                    It.Is<int?>(x => x == expectedParentFolder.Id), 
                    It.Is<string>(x => x == expectedName), 
                    It.Is<string>(x => x == expectedFullPath)));
        }

        [Fact]
        public async Task CreateFolder_should_throw_exception_if_root_path_not_exists()
        {
            string path = "animals/dogs";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(path)))).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileNotFoundException>(async () => await foldersService.CreateFolder(path, "terrier"));
        }

        [Fact]
        public async Task CreateFolder_should_throw_exception_if_folder_already_exists()
        {
            string path = "animals";
            string name = "dogs";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(path)))).ReturnsAsync(null);
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(string.Concat(path, "/", name))))).ReturnsAsync(new FileSystemItem());

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileAlreadyExistsException>(async () => await foldersService.CreateFolder("animals", "dogs"));
        }

        [Fact]
        public async Task MoveFolder_should_move_folder()
        {
            string currentPath = "animals/dogs";
            string newRootPath = "people/workers";
            string newPath = "people/workers/dogs";
            var expectedCurrentItem = new FileSystemItem();
            var expectedNewRootItem = new FileSystemItem() { Id = 4 };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(currentPath)))).ReturnsAsync(expectedCurrentItem);
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(newRootPath)))).ReturnsAsync(expectedNewRootItem);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            var actualFolder = await foldersService.Move(currentPath, newPath);

            foldersRepositoryMock
                .Verify(c => c.ChangeFolder(It.Is<FileSystemItem>(x => 
                    x.ParentId == expectedNewRootItem.Id && x.FullPath == newPath)));
        }

        [Fact]
        public async Task MoveFolder_should_throw_exception_if_current_path_is_not_found()
        {
            string currentPath = "animals/dogs";
            string newPath = "people/workers/dogs";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileNotFoundException>(async () => await foldersService.Move(currentPath, newPath));
        }

        [Fact]
        public async Task MoveFolder_should_throw_exception_if_new_root_path_is_not_found()
        {
            string currentPath = "animals/dogs";
            string newRootPath = "people/workers";
            string newPath = "people/workers/dogs";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(currentPath)))).ReturnsAsync(new FileSystemItem());
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(newRootPath)))).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileNotFoundException>(async () => await foldersService.Move(currentPath, newPath));
        }

        [Fact]
        public async Task RemoveFolder_should_remove_exists_folder()
        {
            var expectedFileSystemItem = new FileSystemItem()
            {
                Id = 1,
                Name = "terrier",
                FullPath = "dogs/terrier"
            };

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.IsAny<string>())).ReturnsAsync(expectedFileSystemItem);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await foldersService.Remove("dogs/terrier");

            foldersRepositoryMock.Verify(c => c.RemoveFolder(It.Is<FileSystemItem>(x => x == expectedFileSystemItem)));
        }

        [Fact]
        public async Task RemoveFolder_should_throw_exception_if_path_not_exists()
        {
            string path = "dogs/terrier";

            var foldersRepositoryMock = new Mock<IFoldersRepository>();
            foldersRepositoryMock.Setup(c => c.GetFileSystemItemByPath(It.Is<string>(x => x.Equals(path)))).ReturnsAsync(null);

            var foldersService = new FoldersService(foldersRepositoryMock.Object);

            await Assert.ThrowsAsync<FileNotFoundException>(async () => await foldersService.CreateFolder(path, "terrier"));
        }
    }
}
