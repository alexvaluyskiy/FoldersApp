using FoldersApp.Core.Exceptions;
using FoldersApp.Repositories;
using FoldersApp.Services;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FoldersApp.Tests
{
    public class FoldersServiceTests
    {
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
