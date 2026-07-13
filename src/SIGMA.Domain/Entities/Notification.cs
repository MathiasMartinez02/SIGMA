using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;

namespace SIGMA.Domain.Entities;

// Notificacion persistida dirigida a un usuario, generada por el proceso en background de vencimientos
public class Notification : AuditableEntity
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public Guid? RelatedEntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    // Crea una notificacion nueva sin leer para el usuario destinatario indicado
    public static Notification Create(
        Guid userId, NotificationType type, string title, string message, Guid? relatedEntityId = null) =>
        new()
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            RelatedEntityId = relatedEntityId,
            IsRead = false
        };

    // Marca la notificacion como leida registrando la fecha y hora
    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
