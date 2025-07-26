using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // This entire controller is for Admins only
public class ReportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("dailysummary")]
    public async Task<IActionResult> GetDailySalesSummary()
    {
        var today = DateTime.UtcNow.Date;

        var totalSales = await _context.Orders
            .Where(o => o.Timestamp.Date == today)
            .SumAsync(o => o.TotalAmount);

        var orderCount = await _context.Orders
            .CountAsync(o => o.Timestamp.Date == today);

        return Ok(new { TotalSales = totalSales, OrderCount = orderCount });
    }
}