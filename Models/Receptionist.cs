using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Receptionist
    {
        public int ReceptionistID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}