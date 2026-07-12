namespace SIGMA.Application.Common.Interfaces;

public interface IWorkOrderNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
