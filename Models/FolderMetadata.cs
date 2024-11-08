using System;

namespace CharacterGrade.Models
{
    public class FolderMetadata
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Path { get; set; }
        public string Metadata { get; set; }
    }
}
