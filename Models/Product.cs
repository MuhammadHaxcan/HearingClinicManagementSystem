using System;

namespace HearingClinicManagementSystem.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime? LastRestocked { get; set; }
    }
}