using System;

namespace HearingClinicManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public Patient Patient { get; set; }
        public int AudiologistID { get; set; }
        public Audiologist Audiologist { get; set; }
        public int? CreatedBy { get; set; } // UserID of creator
        public DateTime Date { get; set; }

        // Reference to specific time slot
        public int TimeSlotID { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public string PurposeOfVisit { get; set; }
        public string Status { get; set; } // "Pending", "Confirmed", etc.
        public decimal Fee { get; set; }
        public bool FollowUpRequired { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}