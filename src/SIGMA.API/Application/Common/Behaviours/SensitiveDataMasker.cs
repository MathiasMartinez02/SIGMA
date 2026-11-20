using System.Reflection;

namespace SIGMA.API.Common.Behaviours;

public static class SensitiveDataMasker
{
    private const string MaskValue = "***";
    private static readonly string[] SensitiveFragments = ["password", "token", "secret"];

    // Propiedades del request con los valores sensibles enmascarados
    public static IReadOnlyDictionary<string, object?> Mask(object request) =>
        request.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .ToDictionary(
                p => p.Name,
                p => IsSensitive(p.Name) ? MaskValue : p.GetValue(request));

    private static bool IsSensitive(string propertyName) =>
        SensitiveFragments.Any(f => propertyName.Contains(f, StringComparison.OrdinalIgnoreCase));
}
