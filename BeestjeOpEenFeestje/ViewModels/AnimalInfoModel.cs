using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class AnimalInfoModel : UpdateAnimalModel
    {
        public List<ReservationModel>? Reservations;
    }
}
