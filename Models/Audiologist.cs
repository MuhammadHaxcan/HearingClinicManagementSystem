namespace HearingClinicManagementSystem.Models
{
    public class Audiologist
    {
        public int AudiologistID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public string Specialization { get; set; }
    }
}