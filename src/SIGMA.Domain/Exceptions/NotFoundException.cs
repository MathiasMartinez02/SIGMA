namespace SIGMA.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"'{entityName}' con id '{key}' no fue encontrado.") { }
}
