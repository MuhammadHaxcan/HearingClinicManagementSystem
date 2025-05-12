using System;

namespace HearingClinicManagementSystem.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
    }
}