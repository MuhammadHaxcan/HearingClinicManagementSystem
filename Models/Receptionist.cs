namespace HearingClinicManagementSystem.Models
{
    public class Receptionist
    {
        public int ReceptionistID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}