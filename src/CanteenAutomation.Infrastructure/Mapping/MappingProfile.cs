using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using CanteenAutomation.Domain.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MenuItem, MenuItemDto>().ReverseMap();

        // Add new mappings for Orders
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.MenuItem.Price));
    }
}