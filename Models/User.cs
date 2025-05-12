using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Required, Phone, StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(20)]
        public string Role { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(255)]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;
    }

}