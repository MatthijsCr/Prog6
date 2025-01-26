using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class UpdateUserModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string Address { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Phone(ErrorMessage = ErrorMessages.InvalidPhoneNumber)]
        public required string PhoneNumber { get; set; }

        [EnumDataType(typeof(CustomerCardType))]
        public CustomerCardType CustomerCard { get; set; }
    }
}
