using Microsoft.AspNetCore.Mvc;
using FoldersApp.Repositories;
using System.Collections.Generic;
using FoldersApp.Models;
using FoldersApp.Services;
using System.Threading.Tasks;

namespace FoldersApp.Controllers
{
    [Route("api/folders")]
    public class FoldersController : Controller
    {
        private FoldersService _foldersService;

        public FoldersController(FoldersService foldersRepository)
        {
            _foldersService = foldersRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(FileSystemItem), 200)]
        public async Task<IActionResult> Get([FromQuery]string path)
        {
            var list = await _foldersService.GetFolder(path);

            return Ok(list);
        }

        [HttpPost("file")]
        [ProducesResponseType(typeof(FileSystemItem), 201)]
        [ProducesResponseType(typeof(KeyValuePair<string, string>), 400)]
        public async Task<IActionResult> PostFile([FromBody]CreateFileSystemItem createFile)
        {
            var file = await _foldersService.CreateFile(createFile.Path, createFile.Name, createFile.Content);

            return Created("", file);
        }

        [HttpPost("folder")]
        [ProducesResponseType(typeof(FileSystemItem), 201)]
        [ProducesResponseType(typeof(KeyValuePair<string, string>), 400)]
        public async Task<IActionResult> PostFolder([FromBody]CreateFileSystemItem createFileSystemItem)
        {
            var folder = await _foldersService.CreateFolder(createFileSystemItem.Path, createFileSystemItem.Name);

            return Created("", folder);
        }

        [HttpPut]
        [ProducesResponseType(typeof(FileSystemItem), 200)]
        [ProducesResponseType(typeof(KeyValuePair<string, string>), 400)]
        public async Task<IActionResult> Put([FromBody]MoveFileSystemItem moveFileSystemItem)
        {
            var folder = await _foldersService.Move(moveFileSystemItem.CurrentPath, moveFileSystemItem.NewPath);

            return Ok(folder);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(KeyValuePair<string, string>), 400)]
        public async Task<IActionResult> Delete([FromQuery]string path)
        {
            await _foldersService.Remove(path);

            return NoContent();
        }
    }
}
