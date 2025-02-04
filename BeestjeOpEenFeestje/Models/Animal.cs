﻿using System.ComponentModel.DataAnnotations;

namespace BeestjeOpEenFeestje.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MaxLengthName)]
        public required string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string Type { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        [Range(0, double.MaxValue, ErrorMessage = ErrorMessages.InvalidPrice)]
        public required double Price { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public required string ImageURL { get; set; }


        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
