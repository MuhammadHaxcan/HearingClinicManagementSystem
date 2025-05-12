namespace HearingClinicManagementSystem.Models
{
    public class InventoryManager
    {
        public int InventoryManagerID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
    }
}