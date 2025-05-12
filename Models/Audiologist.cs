using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class Audiologist
    {
        public int AudiologistID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }

        [StringLength(100)]
        public string Specialization { get; set; }
    }
}