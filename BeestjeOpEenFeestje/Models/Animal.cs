using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MaxLengthName)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public string Type { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Range(0, double.MaxValue, ErrorMessage = ErrorMessages.InvalidPrice)]
        public double Price { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public string ImageURL { get; set; }


        public List<Reservation> Reservations { get; set; }
    }
}
