namespace HearingClinicManagementSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } // Patient/Receptionist/Audiologist/InventoryManager/ClinicManager
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
    }
}