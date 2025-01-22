using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class CreateUserModel : UpdateUserModel
    {

        [EnumDataType(typeof(CustomerCardType))]
        public CustomerCardType? CustomerCard { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string Role { get; set; }
    }
}

