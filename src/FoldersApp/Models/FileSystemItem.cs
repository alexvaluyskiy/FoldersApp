using System.ComponentModel.DataAnnotations;

namespace FoldersApp.Repositories
{
    public class FileSystemItem
    {
        [Key]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public string FullPath { get; set; }

        public FileSystemType Type { get; set; }

        public string FileContent { get; set; }

        public string FileType { get; set; }
    }
}
