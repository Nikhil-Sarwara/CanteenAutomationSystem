
// The namespace needs to be updated. Since it's now in the Domain
// project, a namespace like "CanteenAutomation.Domain.DTOs" makes sense.
namespace CanteenAutomation.Domain.DTOs;

public class MenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}