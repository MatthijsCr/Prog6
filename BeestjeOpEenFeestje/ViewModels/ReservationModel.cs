using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class ReservationModel
    {
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public Reservation Reservation { get; set; }

        public List<Animal> Animals { get; set; }

        [Required(ErrorMessage = ErrorMessages.NoAnimalsSelected)]
        public List<int> SelectedAnimals { get; set; } = new List<int>();
    }
}
