using BeestjeOpEenFeestje.Models;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.ViewModels
{
    public class AccountListModel
    {
        public required string Username { get; set; }
        public required string Address { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string CustomerCard { get; set; }
    }
}
