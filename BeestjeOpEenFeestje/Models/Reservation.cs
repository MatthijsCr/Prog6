using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public DateOnly Date { get; set; }

        public AppUser? AppUser { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public bool IsConfirmed { get; set; }


        public List<Animal> Animals { get; set; } = new List<Animal>();
    }
}
