using System;
using System.Collections.Generic;

namespace ArtStore.DATA.EF.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int OrderProductId { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime PostDate { get; set; }
        public string UserId { get; set; } = null!;

        public virtual OrderProduct OrderProduct { get; set; } = null!;
        public virtual UserDetail User { get; set; } = null!;
    }
}
