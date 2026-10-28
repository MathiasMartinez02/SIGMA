using AutoMapper;
using SIGMA.API.Inventory.DTOs;
using SIGMA.API.Entities;

namespace SIGMA.API.Common.Mappings;

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
