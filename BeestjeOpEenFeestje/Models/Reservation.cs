using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class Reservation
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public AppUser? Customer { get; set; }

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        [Required]
        public List<Animal> Animals { get; set; }
    }
}
