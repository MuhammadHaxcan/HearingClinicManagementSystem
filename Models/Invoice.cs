using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Invoice
    {
        public int InvoiceID { get; set; }

        public int? OrderID { get; set; }
        public Order Order { get; set; }

        public int? AppointmentID { get; set; }
        public Appointment Appointment { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Range(0, 99999.99)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Payment status of the invoice
        /// Options: Pending, Paid
        /// </summary>
        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(20)]
        public string PaymentMethod { get; set; }
    }
}