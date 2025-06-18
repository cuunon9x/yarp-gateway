# Discount Microservice Technical Design Document

**Version:** 1.0  
**Date:** June 9, 2025  
**Author:** System Architect  

## Table of Contents
1. [Introduction](#introduction)
2. [Application Use Cases](#application-use-cases)
3. [Architecture Overview](#architecture-overview)
4. [Core Technologies & Libraries](#core-technologies--libraries)
5. [Design Patterns & Principles](#design-patterns--principles)
6. [Domain Models](#domain-models)
7. [gRPC Endpoints](#grpc-endpoints)
8. [Implementation Details](#implementation-details)
9. [Project Folder Structure](#project-folder-structure)
10. [Database Schema](#database-schema)
11. [Performance Considerations](#performance-considerations)
12. [Implementation Phases](#implementation-phases)
13. [Integration with Basket Service](#integration-with-basket-service)
14. [Summary](#summary)

## 1. Introduction

The Discount Microservice is a crucial component within the overall microservices architecture, designed to manage discount coupons and provide real-time discount calculations to other services, particularly the Basket microservice. The service emphasizes maximum performance and quick response times for synchronous gRPC calls.

### Main Goals:
- **Performance Focus**: Achieve maximum performance and reduce service invocation time
- **Quick Response**: Ensure the Discount microservice responds quickly to synchronous calls from the Basket service
- **Integration**: Seamless integration with Basket microservice for real-time discount application

## 2. Application Use Cases

### Primary Use Case: Add Item into Shopping Cart
When a client adds an item into the shopping cart, the Basket microservice will consume the Discount gRPC service to get the latest discount on that product item.

### CRUD Discount Operations:
- **Get Discount with ProductName**: Retrieve discount information for a specific product
- **Create Discount**: Add a new discount coupon for a product
- **Update Discount**: Modify existing discount details
- **Delete Discount**: Remove a discount coupon

## 3. Architecture Overview

### Overall Architecture:
- **Communication**: gRPC for high-performance inter-service communication
- **Database**: SQLite for embedded, lightweight data storage
- **Architecture Pattern**: **Vertical Slice Architecture combined with CQRS** (to align with Catalog and Basket services)
- **ORM**: Entity Framework Core with SQLite provider

### Internal "Discount" Microservice Architecture:
Similar to the Catalog and Basket Microservices, the DiscountAPI will adhere to a **Vertical Slice Architecture combined with CQRS (Command Query Responsibility Segregation)** for internal organization and maintainability.

- **gRPC Layer**: gRPC endpoints and service definitions
- **Application Layer**: Orchestrates business logic, handles commands and queries via MediatR
- **Domain Layer**: Contains core business entities (Coupon) and business rules
- **Infrastructure Layer**: Database operations, Entity Framework context, external integrations

### CQRS Implementation:
- **Commands**: Write operations (CreateDiscount, UpdateDiscount, DeleteDiscount)
- **Queries**: Read operations (GetDiscount)
- **Handlers**: Process commands and queries using MediatR
- **Vertical Slices**: Each feature (Get, Create, Update, Delete) organized as self-contained slices

## 4. Core Technologies & Libraries

### Framework & Runtime:
- **.NET 8 (ASP.NET Core)**: The foundational framework for building the gRPC service
- **ASP.NET Core**: Web framework for hosting gRPC services

### Database & ORM:
- **SQLite**: Embedded SQL database for lightweight operations
- **Entity Framework Core**: ORM for database operations
- **Microsoft.EntityFrameworkCore.Sqlite**: SQLite provider
- **Microsoft.EntityFrameworkCore.Tools**: EF Core tooling for migrations

### Communication:
- **gRPC**: High-performance RPC framework
- **Grpc.AspNetCore**: gRPC integration for ASP.NET Core

### CQRS & Messaging:
- **MediatR**: Simplifies CQRS implementation and in-process messaging
- **MediatR.Extensions.Microsoft.DependencyInjection**: DI integration for MediatR

### Utilities:
- **Mapster**: High-performance object mapping (consistent with other services)
- **FluentValidation**: Fluent validation library for input validation
- **FluentValidation.DependencyInjectionExtensions**: DI integration for FluentValidation

### Containerization:
- **Docker**: For containerization and consistent deployment

## 5. Design Patterns & Principles

The Discount Microservice will apply the same robust design patterns and principles as the Catalog and Basket Microservices to ensure consistency, maintainability, and scalability across the microservices landscape:

### Patterns Used:
- **CQRS Pattern (Command Query Responsibility Segregation)**: Separates read (queries) and write (commands) operations for better scalability and performance
- **Mediator Pattern**: Facilitates object interaction through MediatR, reducing direct dependencies between objects
- **Repository Pattern**: Data access abstraction to keep persistence concerns outside the domain model
- **Vertical Slice Architecture**: Each feature organized as self-contained slices with all related components
- **ProtoBuf Protocol**: Efficient serialization for gRPC communication

### Principles:
- **Single Responsibility Principle**: Each layer and component has a specific responsibility
- **Dependency Injection (DI)**: Core ASP.NET Core feature for loose coupling and testability
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Performance First**: All decisions prioritize performance and response time for gRPC calls

## 6. Domain Models

### Coupon Entity:
```csharp
public class Coupon
{
    public int Id { get; set; }           // Primary key
    public string ProductName { get; set; } // Product identifier (8 references)
    public string Description { get; set; } // Discount description (3 references)
    public int Amount { get; set; }        // Discount amount (4 references)
}
```

### Key Properties:
- **Id**: Unique identifier for the coupon
- **ProductName**: Associates discount with specific product (most referenced field)
- **Description**: Human-readable description of the discount
- **Amount**: Discount value (could be percentage or fixed amount)

## 7. gRPC Endpoints

| Method (gRPC) | Request URI | Use Cases |
|---------------|-------------|-----------|
| GetDiscount | GetDiscountRequest | Get discount with product name |
| CreateDiscount | CreateDiscountRequest | Create discount |
| UpdateDiscount | UpdateDiscountRequest | Update discount |
| DeleteDiscount | DeleteDiscountRequest | Delete discount |

### gRPC Proto Definitions:
- **GetDiscountRequest**: Contains ProductName
- **GetDiscountResponse**: Returns Coupon details
- **CreateDiscountRequest**: Contains new Coupon data
- **UpdateDiscountRequest**: Contains updated Coupon data
- **DeleteDiscountRequest**: Contains ProductName or Id

## 8. Implementation Details

### 8.1. Infrastructure & Data Interaction
- SQLite database embedded within the application
- Entity Framework Core for ORM operations
- Database migrations for schema management
- Connection string configuration in appsettings.json

### 8.2. CQRS Implementation with MediatR
- **Commands**: Write operations (CreateDiscountCommand, UpdateDiscountCommand, DeleteDiscountCommand)
- **Queries**: Read operations (GetDiscountQuery)
- **Handlers**: Command and Query handlers implementing business logic
- **Pipeline Behaviors**: Cross-cutting concerns like validation using FluentValidation
- **Vertical Slices**: Each feature organized as self-contained slice

### 8.3. Validation & Pipeline Behaviors
- **FluentValidation**: Integrated with MediatR pipeline for request validation
- **MediatR Pipeline Behaviors**: Centralized validation before request processing
- **Input Validation**: All gRPC request validation with fluent interface

### 8.4. Object Mapping & Data Access
- **Mapster**: High-performance object mapping between DTOs and domain models
- **Repository Pattern**: Data access abstraction for testability and loose coupling
- **DiscountContext**: EF Core DbContext for SQLite operations
- **Optimized Queries**: Performance-focused database queries

## 9. Project Folder Structure

```
Discount.Grpc/
├── Features/                         # Vertical Slices (CQRS)
│   ├── GetDiscount/                 # Get Discount Feature
│   │   ├── GetDiscountQuery.cs      # Query definition
│   │   ├── GetDiscountHandler.cs    # Query handler
│   │   └── GetDiscountValidator.cs  # Validation rules
│   ├── CreateDiscount/              # Create Discount Feature
│   │   ├── CreateDiscountCommand.cs # Command definition
│   │   ├── CreateDiscountHandler.cs # Command handler
│   │   └── CreateDiscountValidator.cs # Validation rules
│   ├── UpdateDiscount/              # Update Discount Feature
│   │   ├── UpdateDiscountCommand.cs # Command definition
│   │   ├── UpdateDiscountHandler.cs # Command handler
│   │   └── UpdateDiscountValidator.cs # Validation rules
│   └── DeleteDiscount/              # Delete Discount Feature
│       ├── DeleteDiscountCommand.cs # Command definition
│       ├── DeleteDiscountHandler.cs # Command handler
│       └── DeleteDiscountValidator.cs # Validation rules
├── Models/                          # Domain Layer
│   └── Coupon.cs                   # Domain entity
├── Data/                           # Infrastructure Layer
│   ├── DiscountContext.cs          # EF Core DbContext
│   ├── Repositories/               # Repository implementations
│   │   ├── IDiscountRepository.cs  # Repository interface
│   │   └── DiscountRepository.cs   # Repository implementation
│   └── Migrations/                 # EF Core migrations
├── Services/                       # gRPC Service Layer
│   └── DiscountService.cs          # gRPC service implementation
├── Protos/                         # gRPC Protocol Definitions
│   └── discount.proto              # gRPC service definitions
├── Extensions/                     # Extension methods
│   └── ServiceExtensions.cs        # DI registration extensions
├── Program.cs                      # Application entry point
├── appsettings.json               # Configuration
├── appsettings.Development.json   # Development configuration
└── Discount.Grpc.csproj          # Project file
```

### Folder Organization Philosophy:
- **Features/**: Vertical slice organization with each feature self-contained
- **Models/**: Domain entities and business models
- **Data/**: Infrastructure concerns including EF Core and repositories
- **Services/**: gRPC service implementations
- **Protos/**: Protocol buffer definitions for gRPC contracts

## 10. Database Schema

### Coupon Table:
```sql
CREATE TABLE Coupons (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductName TEXT NOT NULL,
    Description TEXT,
    Amount INTEGER NOT NULL
);
```

### Advantages of SQLite:
- **Embedded**: No separate database server required
- **High Performance**: Suitable for lightweight operations
- **Simple Deployment**: Database file included with application
- **ACID Compliance**: Ensures data consistency

## 11. Performance Considerations

### Optimization Strategies:
- **SQLite Embedded**: Eliminates network latency to database
- **gRPC Protocol**: Binary protocol for efficient communication
- **Async Operations**: Non-blocking I/O operations
- **Connection Pooling**: Efficient database connection management
- **Mapster**: High-performance object mapping library

### Expected Performance:
- Sub-millisecond response times for discount lookups
- Minimal memory footprint
- Quick service startup time
- Efficient resource utilization

## Implementation Phases

1. **Phase 1**: Project setup and basic structure with gRPC template
2. **Phase 2**: Domain model (Coupon) and EF Core setup with SQLite
3. **Phase 3**: gRPC proto definitions and contract setup
4. **Phase 4**: Repository pattern and data access layer
5. **Phase 5**: CQRS implementation with MediatR (Commands and Queries)
6. **Phase 6**: FluentValidation integration with MediatR pipeline
7. **Phase 7**: gRPC service implementation connecting to MediatR
8. **Phase 8**: Object mapping with Mapster
9. **Phase 9**: Database seeding and migrations
10. **Phase 10**: Testing and validation
11. **Phase 11**: Docker containerization
12. **Phase 12**: Integration testing with Basket service

## Integration with Basket Service

### gRPC Client Integration:
The Basket service will consume the Discount service via gRPC calls:

```csharp
// In Basket Service
public async Task<ShoppingCart> StoreBasket(ShoppingCart basket)
{
    foreach (var item in basket.Items)
    {
        // Call Discount service via gRPC
        var discountResponse = await _discountProtoService.GetDiscountAsync(
            new GetDiscountRequest { ProductName = item.ProductName });
        
        // Apply discount to item price
        item.Price = item.Price - discountResponse.Amount;
    }
    
    // Store updated basket with applied discounts
    return await _basketRepository.UpdateBasket(basket);
}
```

## Summary

This comprehensive technical design document provides the foundation for implementing a high-performance Discount microservice that:

- **Aligns with Architecture**: Follows the same Vertical Slice + CQRS patterns as Catalog and Basket services
- **Maintains Consistency**: Uses identical technology stack and design patterns
- **Optimizes Performance**: SQLite + gRPC for maximum speed and minimal latency
- **Ensures Integration**: Seamless gRPC integration with Basket service for real-time discount application
- **Supports Scalability**: Clean architecture supporting future enhancements and maintenance

The design is now ready for step-by-step implementation following the defined phases.
