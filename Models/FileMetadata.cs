using System;

namespace CharacterGrade.Models
{
    public class FileMetadata
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Metadata { get; set; }
        public int FolderMetadataId { get; set; }
        public FolderMetadata FolderMetadata { get; set; }
    }
}
