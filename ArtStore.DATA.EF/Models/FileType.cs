using System;
using System.Collections.Generic;

namespace ArtStore.DATA.EF.Models
{
    public partial class FileType
    {
        public FileType()
        {
            Products = new HashSet<Product>();
        }

        public int FileTypeId { get; set; }
        public string FileTypeName { get; set; } = null!;
        public string FileExtension { get; set; } = null!;
        public string? FileDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
