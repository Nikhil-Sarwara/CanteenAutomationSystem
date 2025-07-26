using System.Security.Claims;
using AutoMapper;
using CanteenAutomation.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ApplicationDbContext _context;

    public OrderController(IOrderService orderService, IMapper mapper, IHubContext<OrderHub> hubContext, ApplicationDbContext context)
    {
        _orderService = orderService;
        _mapper = mapper;
        _hubContext = hubContext;
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")] // Only Admins or Staff can access
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)         // Include the order item details
            .ThenInclude(oi => oi.MenuItem)   // Include the menu item for each order item
            .OrderByDescending(o => o.Timestamp) // Show newest orders first
            .ToListAsync();

        var orderDtos = _mapper.Map<List<OrderDto>>(orders);
        return Ok(orderDtos);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> PlaceOrder([FromBody] OrderRequest orderRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var newOrder = await _orderService.CreateOrderAsync(orderRequest, userId);
        var orderDto = _mapper.Map<OrderDto>(newOrder);

        // Broadcast the new order to all connected clients (e.g., the chef's screen)
        await _hubContext.Clients.All.SendAsync("ReceiveOrder", orderDto);

        return CreatedAtAction(nameof(PlaceOrder), new { id = orderDto.Id }, orderDto);
    }
}