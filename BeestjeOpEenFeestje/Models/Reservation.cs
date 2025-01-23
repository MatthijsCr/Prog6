using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        public int AnimalId { get; set; }

        public int? AppUserId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public DateOnly Date { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
