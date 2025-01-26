using BeestjeOpEenFeestje.Models;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class ReservationPriceModel
    {
        public required Reservation Reservation { get; set; }
        public List<Discount> Discounts { get; set; } = new List<Discount>();
        public double PriceTotal { get; set; }
    }
}
