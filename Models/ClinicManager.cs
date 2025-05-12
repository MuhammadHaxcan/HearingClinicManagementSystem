namespace HearingClinicManagementSystem.Models
{
    public class ClinicManager
    {
        public int ClinicManagerID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}       