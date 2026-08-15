using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ParkLink.Payment.Data;
using ParkLink.Payment.Extensions;
using ParkLink.Payment.Services;
using ParkLink.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<PaymentContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("parklink-paymentdb"),
            sql =>
            {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            }
        );
    });

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddPaymentProviders();

builder.Services.AddPaymentMessaging(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["ParkinLinkPaymentSettings:AuthorityUrl"]!),
                TokenUrl = new Uri(builder.Configuration["ParkinLinkPaymentSettings:TokenUrl"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "paymentapi", "Payment System API" }
                }
            },
        }
    });

    // Apply Scheme globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "paymentapi" }
        }
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["ParkinLinkPaymentSettings:Authority"];
        options.RequireHttpsMetadata = bool.Parse(builder.Configuration["ParkinLinkPaymentSettings:RequireHttpsMetadata"]!);
        options.SaveToken = bool.Parse(builder.Configuration["ParkinLinkPaymentSettings:SaveToken"]!);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromSeconds(0)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "paymentapi");
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.UseParkLinkCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();