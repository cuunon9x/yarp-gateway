using Serilog;
using System.Threading.RateLimiting;
using YarpGateway.Extensions;
using YarpGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/yarp-gateway-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add health checks
builder.Services.AddBackendHealthChecks(builder.Configuration);

// Add Health Checks UI
builder.Services.AddHealthChecksUI(options => 
{
    options.SetEvaluationTimeInSeconds(15);
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.AddHealthCheckEndpoint("Gateway", "/health");
    options.AddHealthCheckEndpoint("Catalog API", "/health/catalog");
    options.AddHealthCheckEndpoint("Basket API", "/health/basket");
    options.AddHealthCheckEndpoint("Discount API", "/health/discount");
    options.AddHealthCheckEndpoint("Discount gRPC", "/health/discount-grpc");
    options.AddHealthCheckEndpoint("Ordering API", "/health/ordering");
})
.AddInMemoryStorage();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter", _ =>
            new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromSeconds(10)
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});

// Add authentication (commented for initial setup - uncomment and configure when needed)
/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
    });
builder.Services.AddAuthorization();
*/

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Use Serilog request logging
app.UseSerilogRequestLogging();

// Use middleware for correlation IDs
app.UseMiddleware<CorrelationIdMiddleware>();

// Use CORS
app.UseCors();

// Use rate limiting
app.UseRateLimiter();

// Map health checks endpoints
app.MapHealthChecksUI();

// Map health checks dashboard
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-api";
});

// Uncomment when authentication is configured
// app.UseAuthentication();
// app.UseAuthorization();

// Map reverse proxy routes
app.MapReverseProxy();

app.Run();
