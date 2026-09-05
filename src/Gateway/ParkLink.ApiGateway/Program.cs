using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ParkLink.ServiceDefaults.Exceptions;
using ParkLink.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Aspire & Service Defaults
builder.AddServiceDefaults();

// Problem Details & Global Exception Handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Identity:Authority"];
        options.Audience = "parklink.gateway";
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = "parklink.gateway",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GatewayAccess", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "parklink.gateway");
    });

    // Vehicle API
    options.AddPolicy("VehicleManagement", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "vehicleapi");
    });

    // Parking API
    options.AddPolicy("ParkingManagement", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "parkingapi");
    });

    // Reservation API
    options.AddPolicy("ReservationManagement", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "reservationapi");
    });

    // Payment API
    options.AddPolicy("PaymentManagement", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "paymentapi");
    });

    // Notification API
    options.AddPolicy("NotificationManagement", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "notificationapi");
    });

    // User API
    options.AddPolicy("UserManagement", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "userapi");
    });

    // Gate API
    options.AddPolicy("GateManagement", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireClaim("scope", "gateapi");
    });
});

// YARP
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

// Error Handling
app.UseExceptionHandler();

// Correlation ID
app.UseParkLinkCorrelationId();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Aspire Default Endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

// Reverse Proxy
app.MapReverseProxy();

app.Run();
