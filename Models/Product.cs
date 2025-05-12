using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required, StringLength(100)]
        public string Manufacturer { get; set; }

        [Required, StringLength(100)]
        public string Model { get; set; }

        [StringLength(500)]
        public string Features { get; set; }

        [Range(0, 9999.99)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }
    }
}