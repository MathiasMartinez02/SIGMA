using MediatR;
using Microsoft.Extensions.Logging;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        => _logger = logger;

    // Log del request sin datos sensibles y del resultado sin contenido
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("SIGMA Request: {Name} {@Request}", requestName, SensitiveDataMasker.Mask(request));

        var response = await next();

        if (response is Result { Failed: true } failed)
            _logger.LogInformation("SIGMA Response: {Name} fallo {@Errors}", requestName, failed.Errors);
        else
            _logger.LogInformation("SIGMA Response: {Name} ok", requestName);

        return response;
    }
}
