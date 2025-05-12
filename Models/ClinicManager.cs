using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class ClinicManager
    {
        public int ClinicManagerID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}