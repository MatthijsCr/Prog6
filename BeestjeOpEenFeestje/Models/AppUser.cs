using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class AppUser : IdentityUser
    {
        public CustomerCardType CustomerCard { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Phone(ErrorMessage = ErrorMessages.InvalidPhoneNumber)]
        public override string? PhoneNumber { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [StringLength(150, ErrorMessage = ErrorMessages.MaxLengthAddress)]
        public string Address { get; set; }

        
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}