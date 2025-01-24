using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class ReservationModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public Reservation Reservation { get; set; }

        public List<Animal> Animals { get; set; }

        public List<string> SelectedAnimals { get; set; }
    }
}
