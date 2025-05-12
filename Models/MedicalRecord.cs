using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class MedicalRecord
    {
        public int RecordID { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int AppointmentID { get; set; }
        public Appointment Appointment { get; set; }

        [Required]
        public int CreatedBy { get; set; }
        public Audiologist Creator { get; set; }

        [Required, StringLength(1000)]
        public string ChiefComplaint { get; set; }

        [StringLength(1000)]
        public string Diagnosis { get; set; }

        [StringLength(2000)]
        public string TreatmentPlan { get; set; }

        [Required]
        public DateTime RecordDate { get; set; } = DateTime.Now;
    }
}