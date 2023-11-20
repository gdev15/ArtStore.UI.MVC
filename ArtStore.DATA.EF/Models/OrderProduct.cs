using System;
using System.Collections.Generic;

namespace ArtStore.DATA.EF.Models
{
    public partial class OrderProduct
    {
        public OrderProduct()
        {
            Reviews = new HashSet<Review>();
        }

        public int OrderProductId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public decimal? ProductPrice { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
