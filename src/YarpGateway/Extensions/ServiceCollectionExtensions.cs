namespace YarpGateway.Extensions;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using HealthChecks.Uris;

public static class ServiceCollectionExtensions
{    // Add health checks for specific backend services
    public static IServiceCollection AddBackendHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();        // Add catalog health check
        var catalogUrl = configuration["BackendServices:Catalog"];
        if (!string.IsNullOrEmpty(catalogUrl))
        {
            builder.AddUrlGroup(
                uri: new Uri(catalogUrl),
                name: "catalog-api",
                tags: new[] { "catalog", "ready" });
        }

        // Add basket health check
        var basketUrl = configuration["BackendServices:Basket"];
        if (!string.IsNullOrEmpty(basketUrl))
        {
            builder.AddUrlGroup(
                uri: new Uri(basketUrl),
                name: "basket-api",
                tags: new[] { "basket", "ready" });
        }        // Add discount health check
        var discountUrl = configuration["BackendServices:Discount"];
        if (!string.IsNullOrEmpty(discountUrl))
        {
            builder.AddUrlGroup(
                uri: new Uri(discountUrl),
                name: "discount-api",
                tags: new[] { "discount", "ready" });
        }

        // Add discount gRPC health check
        var discountGrpcUrl = configuration["BackendServices:DiscountGrpc"];
        if (!string.IsNullOrEmpty(discountGrpcUrl))
        {
            builder.AddUrlGroup(
                uri: new Uri(discountGrpcUrl),
                name: "discount-grpc",
                tags: new[] { "discount-grpc", "ready" });
        }

        // Add ordering health check
        var orderingUrl = configuration["BackendServices:Ordering"];
        if (!string.IsNullOrEmpty(orderingUrl))
        {
            builder.AddUrlGroup(
                uri: new Uri(orderingUrl),
                name: "ordering-api",
                tags: new[] { "ordering", "ready" });
        }
        
        return services;
    }
}
