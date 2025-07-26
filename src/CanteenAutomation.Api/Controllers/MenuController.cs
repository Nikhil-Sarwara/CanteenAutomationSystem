// In: src/CanteenAutomation.Api/Controllers/MenuController.cs
using AutoMapper;
using CanteenAutomation.Domain.DTOs; // <-- Update this using statement
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis; // Add this
using System.Text.Json;

// ... rest of the controller code[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints in this controller require authentication
public class MenuController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDatabase _redisDatabase; // Add this

    // Update the constructor
    public MenuController(ApplicationDbContext context, IMapper mapper, IConnectionMultiplexer redis)
    {
        _context = context;
        _mapper = mapper;
        _redisDatabase = redis.GetDatabase(); // Initialize Redis database
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenu()
    {
        const string cacheKey = "menu";

        // 1. Try to get the menu from the cache
        var cachedMenu = await _redisDatabase.StringGetAsync(cacheKey);

        if (!cachedMenu.IsNullOrEmpty)
        {
            // If found, deserialize and return it
            var menu = JsonSerializer.Deserialize<List<MenuItemDto>>(cachedMenu);
            return Ok(menu);
        }

        // 2. If not in cache, get it from the database
        var menuItemsFromDb = await _context.MenuItems.ToListAsync();
        var menuItemsDto = _mapper.Map<List<MenuItemDto>>(menuItemsFromDb);

        // 3. Store the result in the cache for next time (e.g., for 10 minutes)
        await _redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(menuItemsDto), TimeSpan.FromMinutes(10));

        return Ok(menuItemsDto);
    }

    // POST: api/menu
    // Accessible only by users in the "Admin" role
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MenuItemDto>> AddMenuItem([FromBody] MenuItemDto newMenuItemDto)
    {
        var menuItem = _mapper.Map<MenuItem>(newMenuItemDto);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var createdDto = _mapper.Map<MenuItemDto>(menuItem);
        return CreatedAtAction(nameof(GetMenu), new { id = createdDto.Id }, createdDto);
    }
}