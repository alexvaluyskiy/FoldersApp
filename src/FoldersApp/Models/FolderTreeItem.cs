using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FoldersApp.Repositories
{
    [DataContract]
    public class FolderTreeItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FileSystemType Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<FolderTreeItem> Children { get;set; }
    }
}
