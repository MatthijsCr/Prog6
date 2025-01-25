using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class CreateAnimalModel : ListAnimalModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string ImageURL;
    }
}
