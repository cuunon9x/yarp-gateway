namespace YarpGateway.Extensions;

using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

public static class WebApplicationExtensions
{
    // Configure the health checks endpoints with UI
    public static WebApplication MapHealthChecksUI(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => !check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Map specific service health checks
        app.MapHealthChecks("/health/catalog", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("catalog"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/basket", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("basket"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });        app.MapHealthChecks("/health/discount", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("discount"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/discount-grpc", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("discount-grpc"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ordering", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ordering"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
