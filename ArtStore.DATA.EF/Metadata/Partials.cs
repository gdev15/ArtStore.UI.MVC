using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ArtStore.DATA.EF.Models//.Metadata
{
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { }    

    [ModelMetadataType(typeof(FileTypeMetadata))]
    public partial class FileType { }

    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }

    [ModelMetadataType(typeof(OrderProductMetadata))]
    public partial class OrderProduct { }

    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
        [NotMapped]
        public IFormFile? Image { get; set; }
    }

    [ModelMetadataType(typeof(ReviewMetadata))]
    public partial class Review { }

    [ModelMetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail
    {
        [Display(Name = "Full Name")]
        public string FullName => FirstName + " " + LastName;
    }
}
