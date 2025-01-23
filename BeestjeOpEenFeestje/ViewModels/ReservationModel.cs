using BeestjeOpEenFeestje.Models;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class ReservationModel
    {
        public Reservation Reservation { get; set; }

        public List<Animal> Animals { get; set; }

        public List<string> SelectedAnimals { get; set; }
    }
}
