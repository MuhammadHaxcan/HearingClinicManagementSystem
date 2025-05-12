using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }

        [Required]
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }

        [Range(0.01, 99999.99)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        public int ReceivedBy { get; set; }
        public Receptionist Receiver { get; set; }

        [StringLength(20)]
        public string PaymentMethod { get; set; }
    }
}
