namespace SIGMA.Application.Clients.DTOs;

public class ClientDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public string TaxId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string? ContactPerson { get; init; }
    public string? ContactPhone { get; init; }
    public bool IsActive { get; init; }
    public int AircraftCount { get; init; }
    public int TotalWorkOrders { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class ClientDetailDto : ClientDto
{
    public IList<ClientAircraftDto> Aircraft { get; init; } = [];
    public IList<ClientWorkOrderDto> RecentWorkOrders { get; init; } = [];
}

public class ClientAircraftDto
{
    public Guid Id { get; init; }
    public string Registration { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalFlightHours { get; init; }
}

public class ClientWorkOrderDto
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
