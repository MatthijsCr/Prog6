using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class CreateAnimalModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string ImageURL;

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MaxLengthName)]
        public required string Name;

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string Type;

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Range(0, double.MaxValue, ErrorMessage = ErrorMessages.InvalidPrice)]
        public required double Price;
    }
}
