namespace SIGMA.API.Enums;

// Estados posibles de un turno solicitado por un cliente antes de convertirse en OT
public enum AppointmentStatus
{
    Solicitado,
    Confirmado,
    Cancelado,
    ConvertidoOT
}
