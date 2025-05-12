using System;

namespace HearingClinicManagementSystem.Models
{
    public class InventoryTransaction
    {
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public string TransactionType { get; set; } // Restock/Sale/Adjustment
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public int ProcessedBy { get; set; } // InventoryManager UserID
    }
}