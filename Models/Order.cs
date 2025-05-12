using System.Collections.Generic;
using System;

namespace HearingClinicManagementSystem.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int PatientID { get; set; }
        public Patient Patient { get; set; }
        public int? ProcessedBy { get; set; } // InventoryManager UserID
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string deliveryAddress { get; set; } // gets from Patient
        public string Status { get; set; } // Cart/Pending/Confirmed/Delivered/Cancelled
        public List<OrderItem> OrderItems { get; set; }
    }
}