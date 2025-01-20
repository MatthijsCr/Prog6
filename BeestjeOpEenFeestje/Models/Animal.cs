using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class Animal
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string ImageURL { get; set; }
    }
}
