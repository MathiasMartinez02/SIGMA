using AutoMapper;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Common.Mappings;

public class InventoryMappingProfile : Profile
{
    public InventoryMappingProfile()
    {
        CreateMap<InventoryItem, InventoryItemDto>();
        CreateMap<InventoryMovement, InventoryMovementDto>()
            .ForMember(d => d.PerformedByName, o => o.MapFrom(s =>
                s.PerformedBy != null ? s.PerformedBy.FullName : string.Empty));
    }
}
