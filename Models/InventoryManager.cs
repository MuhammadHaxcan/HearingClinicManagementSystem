using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class InventoryManager
    {
        public int InventoryManagerID { get; set; }

        [Required]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}