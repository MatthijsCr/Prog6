using BeestjeOpEenFeestje.Models;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class ReservationPriceModel
    {
        public required Reservation Reservation { get; set; }
        public List<string> Discounts { get; set; } = new List<string>();
        public double PriceTotal { get; set; }
    }
}
