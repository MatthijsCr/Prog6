using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class AnimalInfoModel
    {
        public required int Id;
        public required string Name;
        public required string Type;

        public required double Price;
        public required string ImageURL;


        public List<ReservationModel>? Reservations;
    }
}
