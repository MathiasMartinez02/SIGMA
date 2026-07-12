using Microsoft.OpenApi.Models;

namespace SIGMA.API.Extensions;

public static class SwaggerExtensions
{
    public static void AddSigmaSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SIGMA API",
                Version = "v1",
                Description = "Sistema Integrado de Gestión de Mantenimiento Aeronáutico",
                Contact = new OpenApiContact
                {
                    Name = "SIGMA Team",
                    Email = "dev@sigma.aero"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingrese el token JWT. Ejemplo: Bearer {token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
            c.DocInclusionPredicate((_, _) => true);
        });
    }
}
