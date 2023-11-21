using System.ComponentModel.DataAnnotations;

namespace ArtStore.UI.MVC.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

    }
}
