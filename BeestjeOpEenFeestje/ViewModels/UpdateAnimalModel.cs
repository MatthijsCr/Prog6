using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class UpdateAnimalModel : CreateAnimalModel
    {
        [Required]
        public required int Id { get; set; }
    }
}
