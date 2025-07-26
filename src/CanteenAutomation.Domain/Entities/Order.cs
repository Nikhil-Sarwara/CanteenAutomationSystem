using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } // Foreign key to the User
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}