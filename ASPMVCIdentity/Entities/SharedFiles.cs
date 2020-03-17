using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASPMVCIdentity.Entities
{
    public partial class SharedFiles
    {
        
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }

    }
}
