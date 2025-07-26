using Microsoft.EntityFrameworkCore;
using Serilog;

public class DailySalesJob
{
    private readonly ApplicationDbContext _context;

    public DailySalesJob(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Execute()
    {
        var today = DateTime.UtcNow.Date;
        var sales = await _context.Orders
            .Where(o => o.Timestamp.Date == today)
            .SumAsync(o => o.TotalAmount);

        // For now, we'll just log the result.
        // In a real app, you might save this to a report table or send an email.
        Log.Information("Daily Sales Report for {Date}: ${SalesTotal}", today.ToShortDateString(), sales);
    }
}