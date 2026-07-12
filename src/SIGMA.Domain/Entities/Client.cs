using SIGMA.Domain.Common;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

public class Client : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string BusinessName { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string? ContactPhone { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Aircraft> Aircraft { get; private set; } = [];
    public ICollection<WorkOrder> WorkOrders { get; private set; } = [];

    private Client() { }

    public static Client Create(
        string name, string businessName, string taxId,
        string email, string phone, string address,
        string city, string province,
        string? contactPerson = null, string? contactPhone = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("El nombre del cliente es requerido.");
        if (string.IsNullOrWhiteSpace(taxId)) throw new DomainException("El CUIT es requerido.");

        return new Client
        {
            Name = name.Trim(),
            BusinessName = businessName.Trim(),
            TaxId = taxId.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            Address = address.Trim(),
            City = city.Trim(),
            Province = province.Trim(),
            ContactPerson = contactPerson,
            ContactPhone = contactPhone
        };
    }

    public void Update(string name, string businessName, string taxId, string email, string phone,
        string address, string city, string province, string? contactPerson, string? contactPhone)
    {
        Name = name.Trim();
        BusinessName = businessName.Trim();
        TaxId = taxId.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        Address = address.Trim();
        City = city.Trim();
        Province = province.Trim();
        ContactPerson = contactPerson;
        ContactPhone = contactPhone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
}
