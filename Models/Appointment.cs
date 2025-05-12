using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int AudiologistID { get; set; }
        public Audiologist Audiologist { get; set; }

        public int? CreatedBy { get; set; }
        public User Creator { get; set; } //can be the receptionist or the patient himself

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int TimeSlotID { get; set; }
        public TimeSlot TimeSlot { get; set; }

        [Required, StringLength(500)]
        public string PurposeOfVisit { get; set; }

        /// <summary>
        /// Current status of the appointment
        /// Options: Pending, Confirmed, Completed, Cancelled
        /// </summary>
        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Range(0, 9999.99)]
        public decimal Fee { get; set; }

        public bool FollowUpRequired { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}