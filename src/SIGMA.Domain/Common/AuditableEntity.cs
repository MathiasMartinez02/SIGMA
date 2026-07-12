namespace SIGMA.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedById { get; set; }
    public bool IsDeleted { get; protected set; }
    public void SoftDelete() => IsDeleted = true;
}
