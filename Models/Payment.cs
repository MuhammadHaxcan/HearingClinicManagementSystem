using System;

namespace HearingClinicManagementSystem.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int ReceivedBy { get; set; } // Receptionist UserID
    }
}
