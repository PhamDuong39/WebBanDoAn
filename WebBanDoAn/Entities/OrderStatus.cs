using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.Entities
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }

        public string? StatusName { get; set; }
        public virtual  IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
