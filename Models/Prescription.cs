using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Prescription
    {
        public int PrescriptionID { get; set; }

        [Required]
        public int AppointmentID { get; set; }
        public Appointment Appointment { get; set; }

        public int? ProductID { get; set; }
        public Product Product { get; set; }

        [Required]
        public int PrescribedBy { get; set; }
        public Audiologist Prescriber { get; set; }

        [Required]
        public DateTime PrescribedDate { get; set; } = DateTime.Now;
    }
}