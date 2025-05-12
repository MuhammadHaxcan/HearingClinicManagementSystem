using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        public int? ProcessedBy { get; set; }
        public InventoryManager Processor { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Range(0, 99999.99)]
        public decimal TotalAmount { get; set; }

        [Required, StringLength(200)]
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Current status of the order
        /// Options: Cart, Pending, Confirmed, Cancelled
        /// </summary>
        [Required, StringLength(20)]
        public string Status { get; set; } = "Cart";

        public ICollection<OrderItem> OrderItems { get; set; }
    }   
}