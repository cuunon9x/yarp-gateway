# YARP API Gateway Technical Design Document

**Version:** 1.0  
**Date:** June 17, 2025  
**Author:** System Architect  

## Table of Contents
1. [Introduction](#introduction)
2. [Architecture Overview](#architecture-overview)
3. [Core Technologies & Libraries](#core-technologies--libraries)
4. [Design Patterns & Principles](#design-patterns--principles)
5. [Gateway Configuration](#gateway-configuration)
6. [Routing Strategy](#routing-strategy)
7. [Implementation Details](#implementation-details)
8. [Security & Authentication](#security--authentication)
9. [Load Balancing & Health Checks](#load-balancing--health-checks)
10. [Monitoring & Logging](#monitoring--logging)
11. [Project Structure](#project-structure)
12. [Containerization](#containerization)

## 1. Introduction

The YARP API Gateway serves as the single entry point for all client applications in the microservices ecosystem. It routes requests to the appropriate backend services (Catalog, Basket, Discount, and Ordering) while providing cross-cutting concerns such as authentication, rate limiting, and monitoring.

### Main Goals:
- **Centralized Entry Point**: Single point of access for all microservices
- **Request Routing**: Intelligent routing based on URL patterns
- **Service Abstraction**: Hide internal service topology from clients
- **Cross-Cutting Concerns**: Authentication, logging, rate limiting, CORS
- **High Performance**: Minimize latency while maximizing throughput

## 2. Architecture Overview

### Overall Architecture:
```
[Client Apps] → [YARP Gateway] → [Microservices]
     ↓              ↓                   ↓
Shopping.Web → API Gateway → Catalog, Basket, Discount, Ordering
```

### Gateway Responsibilities:
- **Request Routing**: Route requests to appropriate microservices
- **Load Balancing**: Distribute load across service instances
- **Health Checks**: Monitor service availability
- **Authentication**: Centralized security layer
- **Rate Limiting**: Protect backend services
- **CORS**: Handle cross-origin requests
- **Request/Response Transformation**: Modify headers, add correlation IDs

## 3. Core Technologies & Libraries

### Framework & Runtime:
- **.NET 8 (ASP.NET Core)**: The foundational framework
- **Microsoft.ReverseProxy (YARP)**: Core reverse proxy functionality

### Cross-Cutting Concerns:
- **Serilog**: Structured logging with various sinks
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication
- **Microsoft.Extensions.Diagnostics.HealthChecks**: Health monitoring
- **Microsoft.AspNetCore.RateLimiting**: Rate limiting functionality

### Containerization:
- **Docker**: For containerization and deployment

## 4. Design Patterns & Principles

### Patterns Used:
- **Gateway Pattern**: Single entry point for microservices
- **Proxy Pattern**: YARP acts as a proxy to backend services
- **Circuit Breaker Pattern**: Fault tolerance for service calls
- **Bulkhead Pattern**: Isolate resources for different services
- **Health Check Pattern**: Monitor service availability

### Principles:
- **Single Responsibility**: Gateway focuses on routing and cross-cutting concerns
- **High Availability**: Fault tolerance and graceful degradation
- **Performance First**: Minimize latency overhead
- **Configuration-Driven**: Route configuration via appsettings.json
- **Observability**: Comprehensive logging and monitoring

## 5. Gateway Configuration

### Route Configuration Structure:
Routes are configured in `appsettings.json` with the following pattern:
- **RouteId**: Unique identifier for each route
- **ClusterId**: Backend service cluster identifier
- **Match**: URL matching patterns
- **Transforms**: Request/response modifications

### Service Discovery:
- Static configuration in appsettings.json
- Support for dynamic service discovery (future enhancement)
- Health check integration for service availability

## 6. Routing Strategy

### URL Patterns:
```
/api/catalog/**     → Catalog Microservice
/api/basket/**      → Basket Microservice
/api/discount/**    → Discount Microservice (gRPC)
/api/ordering/**    → Ordering Microservice
/health/**          → Health Check Endpoints
```

### Route Priorities:
1. Exact match routes
2. Prefix match routes
3. Wildcard routes
4. Default/fallback routes

## 7. Implementation Details

### 7.1. Startup Configuration
- Configure YARP services and middleware
- Set up authentication and authorization
- Configure health checks for backend services
- Set up rate limiting policies

### 7.2. Route Configuration
- Define routes for each microservice
- Configure load balancing strategies
- Set up health check policies
- Configure request/response transforms

### 7.3. Middleware Pipeline
```
Request → CORS → Authentication → Rate Limiting → YARP Proxy → Response
```

### 7.4. Error Handling
- Custom error handling middleware
- Graceful degradation for service failures
- Circuit breaker implementation
- Detailed error logging

## 8. Security & Authentication

### Authentication Strategy:
- **JWT Bearer Token**: Primary authentication mechanism
- **API Key**: For service-to-service communication
- **CORS**: Configure allowed origins for web clients

### Authorization:
- Route-based authorization policies
- Role-based access control
- Custom authorization handlers

## 9. Load Balancing & Health Checks

### Load Balancing:
- **Round Robin**: Default strategy for service instances
- **Least Connections**: For connection-sensitive services
- **Health-Based**: Route only to healthy instances

### Health Checks:
- Gateway health endpoint: `/health`
- Backend service health monitoring
- Automatic service discovery updates
- Circuit breaker integration

## 10. Monitoring & Logging

### Logging Strategy:
- **Structured Logging**: Using Serilog
- **Request/Response**: Log all gateway traffic
- **Performance Metrics**: Response times, error rates
- **Correlation IDs**: Track requests across services

### Monitoring:
- Health check dashboards
- Performance metrics collection
- Error rate monitoring
- Service availability tracking

## 11. Project Structure

```
YarpGateway/
├── Program.cs                      # Application entry point
├── appsettings.json               # Configuration including routes
├── appsettings.Development.json   # Development-specific config
├── appsettings.Production.json    # Production-specific config
├── Configuration/                 # Configuration classes
│   ├── AuthenticationConfig.cs    # Auth configuration
│   └── RateLimitConfig.cs        # Rate limiting configuration
├── Middleware/                    # Custom middleware
│   ├── CorrelationIdMiddleware.cs # Request correlation
│   ├── ErrorHandlingMiddleware.cs # Global error handling
│   └── RequestLoggingMiddleware.cs # Request/response logging
├── HealthChecks/                  # Health check implementations
│   ├── CatalogHealthCheck.cs     # Catalog service health
│   ├── BasketHealthCheck.cs      # Basket service health
│   ├── DiscountHealthCheck.cs    # Discount service health
│   └── OrderingHealthCheck.cs    # Ordering service health
├── Extensions/                    # Extension methods
│   ├── ServiceCollectionExtensions.cs # DI configuration
│   └── WebApplicationExtensions.cs    # Middleware configuration
└── Dockerfile                    # Container configuration
```

## 12. Containerization

### Docker Configuration:
- Multi-stage build for optimized image size
- Health check integration in container
- Environment-specific configuration
- Port mapping for gateway endpoint

### Container Networking:
- Bridge network for inter-service communication
- Port exposure for external access
- Service name resolution via Docker DNS

## 13. Performance Considerations

### Optimization Strategies:
- **Connection Pooling**: Reuse connections to backend services
- **Request Buffering**: Minimize memory usage for large requests
- **Response Compression**: Reduce bandwidth usage
- **Caching**: Cache static responses where appropriate

### Monitoring Metrics:
- Request throughput (requests/second)
- Response latency (p50, p95, p99)
- Error rates by service
- Connection pool utilization

## 14. Deployment Strategy

### Development Environment:
- Docker Compose for local development
- Hot reload for configuration changes
- Development-specific routing

### Production Environment:
- Kubernetes deployment with multiple replicas
- Load balancer in front of gateway instances
- Blue-green deployment strategy
- Configuration via environment variables

## 15. Future Enhancements

### Planned Features:
- **Dynamic Service Discovery**: Consul or similar integration
- **GraphQL Gateway**: Support for GraphQL endpoints
- **WebSocket Support**: Real-time communication proxy
- **Advanced Analytics**: Detailed traffic analytics
- **Custom Plugins**: Extensible middleware architecture

## 16. Integration Points

### With Microservices:
- **Catalog API**: HTTP REST endpoints
- **Basket API**: HTTP REST endpoints
- **Discount API**: gRPC service (HTTP/2)
- **Ordering API**: HTTP REST endpoints

### With Client Applications:
- **Shopping.Web**: Primary web client
- **Mobile Apps**: Future mobile applications
- **Third-party Integrations**: External API consumers

## 17. Configuration Examples

### Basic Route Configuration:
```json
{
  "ReverseProxy": {
    "Routes": {
      "catalog-route": {
        "ClusterId": "catalog-cluster",
        "Match": {
          "Path": "/api/catalog/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "catalog-cluster": {
        "Destinations": {
          "catalog-api": {
            "Address": "http://catalog-api:8080"
          }
        }
      }
    }
  }
}
```

This technical design provides a comprehensive foundation for implementing the YARP API Gateway as the central component of your microservices architecture.
