using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Extensions;
using ParkLink.ServiceDefaults.Exceptions;
using ParkLink.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<ReservationContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "parklink-reservationdb"),
        sql =>
        {
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    }
);

builder.Services.AddReservationMessaging(builder.Configuration);
builder.Services.AddReservationServices();

builder.Services.AddControllers();

List<string> urlList = builder.Configuration.GetSection("WebClients:Links").Get<List<string>>()!;
string[] clientUrls = urlList!.Select(i => i.ToString()).ToArray();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(clientUrls)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
                AuthorizationUrl = new Uri(builder.Configuration["ParkinLinkReservationSettings:AuthorityUrl"]!),
                TokenUrl = new Uri(builder.Configuration["ParkinLinkReservationSettings:TokenUrl"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "reservationapi", "Reservation System API" }
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
            new[] { "reservationapi" }
        }
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["ParkinLinkReservationSettings:Authority"];
        options.RequireHttpsMetadata = bool.Parse(builder.Configuration["ParkinLinkReservationSettings:RequireHttpsMetadata"]!);
        options.SaveToken = bool.Parse(builder.Configuration["ParkinLinkReservationSettings:SaveToken"]!);
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
        policy.RequireClaim("scope", "reservationapi");
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reservation System API");
        options.OAuthClientId(builder.Configuration["ParkinLinkReservationSettings:ApiName"]);
        options.OAuthRealm(" ");
        options.OAuthAppName(" ");
        options.OAuthUsePkce();
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(builder.Configuration["AppSettings:Folder"] + "/swagger/v1/swagger.json", "Reservation System API");
        options.OAuthClientId(builder.Configuration["ParkinLinkReservationSettings:ApiName"]);
        options.OAuthRealm(" ");
        options.OAuthAppName(" ");
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseParkLinkCorrelationId();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
