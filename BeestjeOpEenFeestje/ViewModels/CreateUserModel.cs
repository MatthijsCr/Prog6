using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class CreateUserModel : UpdateUserModel
    {

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string Role { get; set; }
    }
}

