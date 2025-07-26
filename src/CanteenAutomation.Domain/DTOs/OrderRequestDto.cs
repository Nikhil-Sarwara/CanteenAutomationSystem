public class OrderRequest
{
    public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
}

public class OrderItemRequest
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}