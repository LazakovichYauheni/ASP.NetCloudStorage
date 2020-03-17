using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPMVCIdentity
{
    public partial class UploadFiles
    {
        [NotMapped]
        public bool isChecked { get; set; }
        public int Id { get; set; }
        public string FileId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; } 
    }

    public class FileModel
    {
        public List<UploadFiles> Files { get; set; }
    }
    
}
