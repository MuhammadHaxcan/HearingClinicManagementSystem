using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class InventoryTransaction
    {
        public int TransactionID { get; set; }

        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// Type of inventory transaction
        /// Options: Restock, Sale, Adjustment
        /// </summary>
        [Required, StringLength(20)]
        public string TransactionType { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public int ProcessedBy { get; set; }
        public InventoryManager Processor { get; set; }
    }
}