using System.ComponentModel.DataAnnotations.Schema;

namespace BeestjeOpEenFeestje.Models
{
    public class Discount(string name, int amount)
    {
        public string Name { get; set; } = name;
        public int Amount { get; set; } = amount;
    }
}
