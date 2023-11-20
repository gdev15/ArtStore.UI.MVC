using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtStore.DATA.EF.Models//.Metadata
{
    public class CategoryMetadata
    {
        //[Key]
        //public int CategoryId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(50)]
        [Display(Name = "Category")]
        [Unicode(false)]
        public string CategoryName { get; set; } = null!;

        [StringLength(500)]
        [Display(Name = "Description")]
        [Unicode(false)]
        [UIHint("MultilineText")]
        public string? CategoryDescription { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; }
    }

    public class FileTypeMetadata
    {
        //public int FileTypeId { get; set; }
        [Required(ErrorMessage = "* Required")]
        [StringLength(50)]
        [Display(Name = "File Type")]
        public string FileTypeName { get; set; } = null!;
        [Required(ErrorMessage = "* Required")]
        [StringLength(5)]
        [Display(Name = "File Extension")]
        public string FileExtension { get; set; } = null!;

        [StringLength(100)]
        [Display(Name = "File Description")]
        [Unicode(false)]
        [UIHint("MultilineText")]
        public string? FileDescription { get; set; }
    }

    public class OrderMetadata
    {
        //[Key]
        //[Display(Name = "Order ID")]
        //public int OrderId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "* Required")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "* Required")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; } = null!;
    }

    public class OrderProductMetadata
    {
        [Required(ErrorMessage = "* Required")]
        public int OrderProductId { get; set; }
        [Required(ErrorMessage = "* Required")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "* Required")]
        public int OrderId { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [DefaultValue(0)]
        public decimal? ProductPrice { get; set; }
    }

    public class ProductMetadata
    {
        //public int ProductId { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Art")]
        public string ProductName { get; set; } = null!;
        [DisplayFormat(NullDisplayText = "No description given")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [StringLength(100)]
        public string? ProductDescription { get; set; }
        [Range(0, (double)decimal.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [DefaultValue(0)]
        public decimal? ProductPrice { get; set; }


        [Display(Name = "Discontinued?")]
        public bool IsDiscontinued { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [StringLength(75)]
        [Display(Name = "Image")]
        public string? ProductImage { get; set; }
        [Display(Name = "File Type")]
        public int? FileTypeId { get; set; }
    }

    public class ReviewMetadata
    {
        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Review")]
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Product")]
        public int OrderProductId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [Display(Name = "Rating")]
        public float Rating { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DisplayFormat(NullDisplayText = "No description given")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string Comment { get; set; } = null!;

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Post Date")]
        public DateTime PostDate { get; set; }

        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; } = null!;
    }

    public class UserDetailMetadata
    {
        //public string UserId { get; set; } = null!;
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [Display(Name = "Email")]
        [StringLength(100)]
        public string EmailAddress { get; set; } = null!;
        [StringLength(24)]

        [DisplayFormat(NullDisplayText = "[None]", ApplyFormatInEditMode = false, DataFormatString = "{0:(###) ###-####")]
        public string? Phone { get; set; }
    }
}
