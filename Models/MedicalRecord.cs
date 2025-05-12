using System;

namespace HearingClinicManagementSystem.Models
{
    public class MedicalRecord
    {
        public int RecordID { get; set; }
        public int PatientID { get; set; }
        public Patient Patient { get; set; }
        public int AppointmentID { get; set; }
        public Appointment Appointment { get; set; }
        public int CreatedBy { get; set; } // Audiologist UserID
        public string ChiefComplaint { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public DateTime RecordDate { get; set; }
    }
}