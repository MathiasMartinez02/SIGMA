namespace SIGMA.API.Common.Interfaces;

public interface IWorkOrderNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}
