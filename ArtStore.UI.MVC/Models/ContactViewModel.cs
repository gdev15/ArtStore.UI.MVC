using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

//INSTRUCTOR BLOCK 5 - 3.A
namespace ArtStore.Models
{
    [Keyless] //There is no primary key cause there is no DB table so explicitly state it
    public class ContactViewModel
    {

        [Required(ErrorMessage = "*Name is required*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Email is required*")]
        [DataType(DataType.EmailAddress)] //Certain formatting is expected (@ symbol, .com, etc.)
        public string Email { get; set; }

        [Required(ErrorMessage = "*Subject is required*")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "*Message is required*")]
        [DataType(DataType.MultilineText)] //MultilineText denotes this field is larger than a standard textbox (<input> => <textarea>)
        public string Message { get; set; }

    }
}

