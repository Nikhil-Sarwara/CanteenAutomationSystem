using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(OrderRequest orderRequest, string userId);
}

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(OrderRequest orderRequest, string userId)
    {
        var newOrder = new Order
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Status = "Pending",
            TotalAmount = 0
        };

        decimal total = 0;
        foreach (var item in orderRequest.Items)
        {
            var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
            if (menuItem == null)
            {
                throw new Exception($"Menu item with ID {item.MenuItemId} not found.");
            }

            newOrder.OrderItems.Add(new OrderItem
            {
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity
            });
            total += menuItem.Price * item.Quantity;
        }

        newOrder.TotalAmount = total;
        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();

        return newOrder;
    }
}