using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string Address { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [EnumDataType(typeof(CustomerCardType))]
        public CustomerCardType? CustomerCard { get; set; }
    }
}

