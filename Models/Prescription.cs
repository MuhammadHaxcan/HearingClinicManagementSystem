using System;

namespace HearingClinicManagementSystem.Models
{
    public class Prescription
    {
        public int PrescriptionID { get; set; }
        public int AppointmentID { get; set; }
        public Appointment Appointment { get; set; }
        public int? ProductID { get; set; }
        public Product Product { get; set; }
        public int PrescribedBy { get; set; } // Audiologist UserID
        public DateTime PrescribedDate { get; set; }
    }
}