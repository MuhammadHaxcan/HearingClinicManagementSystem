using System.ComponentModel.DataAnnotations;

namespace HearingClinicManagementSystem.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }
        public Order Order { get; set; }

        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Range(0, 9999.99)]
        public decimal UnitPrice { get; set; }
    }
}