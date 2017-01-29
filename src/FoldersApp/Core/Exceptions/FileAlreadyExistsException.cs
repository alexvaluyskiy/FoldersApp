using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoldersApp.Core.Exceptions
{
    public class FileAlreadyExistsException : Exception
    {
        public FileAlreadyExistsException(string folderName)
        {
            FolderName = folderName;
        }

        public string FolderName { get; }
    }
}
