using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
