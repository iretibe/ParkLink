using Microsoft.EntityFrameworkCore;
using ParkLink.Gate.Data;
using ParkLink.Gate.Extensions;
using ParkLink.ServiceDefaults.Exceptions;
using ParkLink.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<GateContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("parklink-gatedb"),
            sql =>
            {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            }
        );
    }
);

builder.Services.AddControllers();

builder.Services.AddGateApplication();

builder.Services.AddGateHttpClients(builder.Configuration);

builder.Services.AddGateMessaging(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "./v1/swagger.json",
            "ParkLink Gate API v1"
        );

        options.OAuthClientId(
            builder.Configuration["Swagger:OAuthClientId"]);

        options.OAuthUsePkce();

        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseParkLinkCorrelationId();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();
