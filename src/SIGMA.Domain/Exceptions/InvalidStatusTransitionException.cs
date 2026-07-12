namespace SIGMA.Domain.Exceptions;

public class InvalidStatusTransitionException : DomainException
{
    public InvalidStatusTransitionException(string from, string to)
        : base($"Transición de estado inválida: {from} → {to}") { }
}
