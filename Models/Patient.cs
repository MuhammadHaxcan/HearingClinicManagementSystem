using System;
using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Patient
    {
        public int PatientID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; }
    }
}