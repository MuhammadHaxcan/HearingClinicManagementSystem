using System;

namespace HearingClinicManagementSystem.Models
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public int? OrderID { get; set; }
        public Order Order { get; set; }
        public int? AppointmentID { get; set; }
        public Appointment Appointment { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; } = "COD";
    }
}