using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkLink.Identity.Configurations;
using ParkLink.Identity.Data;
using ParkLink.Identity.Helpers;
using ParkLink.Identity.Models;
using ParkLink.Identity.Services;
using ParkLink.Identity.Services.Emails;
using Serilog;

namespace ParkLink.Identity.Extensions
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddSameSiteCookiePolicy();

            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            var connectionString = GetIdentityConnectionString(builder.Configuration);
            
            builder.Services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        connectionString,
                        sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });
                });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services
                .AddIdentityServer(options =>
                {
                    options.IssuerUri = builder.Configuration["IdentityServer:IssuerUri"];
                    options.UserInteraction.LogoutUrl = "/Account/Logout";
                        
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.EmitStaticAudienceClaim = true;

                    //Token Lifetimes
                    options.Authentication.CookieLifetime = TimeSpan.FromMinutes(60); //Set cookie lifetime
                    options.Authentication.CookieSlidingExpiration = true; //Enable sliding expiration=
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddConfigurationStoreCache()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));

                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600;
                })
                .AddAspNetIdentity<ApplicationUser>();

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedProto |
                    ForwardedHeaders.XForwardedHost;
            });

            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<ApplicationDbContext>()
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

            builder.Services.AddScoped<EmailSenderHelper>();

            builder.Services.AddScoped<IProfileService, PermissionProfileService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddHealthChecks();

            builder.Services
                .AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.ClientId =
                        builder.Configuration["Authentication:Google:ClientId"]
                        ?? throw new InvalidOperationException("Google ClientId is not configured.");

                    options.ClientSecret =
                        builder.Configuration["Authentication:Google:ClientSecret"]
                        ?? throw new InvalidOperationException("Google ClientSecret is not configured.");

                    options.SignInScheme =
                        IdentityServerConstants.ExternalCookieAuthenticationScheme;
                })
                .AddFacebook("Facebook", options =>
                {
                    options.ClientId =
                        builder.Configuration["Authentication:Facebook:ClientId"]
                        ?? throw new InvalidOperationException("Facebook ClientId is not configured.");

                    options.ClientSecret =
                        builder.Configuration["Authentication:Facebook:ClientSecret"]
                        ?? throw new InvalidOperationException("Facebook ClientSecret is not configured.");

                    options.SignInScheme =
                        IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });

            return builder.Build();
        }

        private static string? GetIdentityConnectionString(IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("parklink-identitydb");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            connectionString =
                Environment.GetEnvironmentVariable(
                    "PARKLINK_IDENTITY_CONNECTION");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            throw new InvalidOperationException(
                "No ParkLink Identity database connection string was found. " +
                "Expected Aspire connection string 'parklink-identitydb' " +
                "or environment variable 'PARKLINK_IDENTITY_CONNECTION'.");
        }

        public static async Task<WebApplication> ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                await InitializeDatabase(app);
            }

            // Create scope for seeding
            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeed.SeedAsync(scope.ServiceProvider, app.Configuration);
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseForwardedHeaders();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapHealthChecks("/health");

            app.MapRazorPages()
                .RequireAuthorization();

            return app;
        }

        private static async Task InitializeDatabase(WebApplication app)
        {
            using var serviceScope = app.Services
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var services = serviceScope.ServiceProvider;

            var applicationDbContext =
                services.GetRequiredService<ApplicationDbContext>();

            await applicationDbContext.Database.MigrateAsync();

            var persistedGrantDbContext =
                services.GetRequiredService<PersistedGrantDbContext>();

            await persistedGrantDbContext.Database.MigrateAsync();

            var configurationDbContext =
                services.GetRequiredService<ConfigurationDbContext>();

            await configurationDbContext.Database.MigrateAsync();

            SeedIdentityServerConfiguration(configurationDbContext, app.Configuration);
        }

        private static void SeedIdentityServerConfiguration(
            ConfigurationDbContext context, IConfiguration configuration)
        {
            foreach (var client in Config.Clients(configuration))
            {
                if (!context.Clients.Any(c => c.ClientId == client.ClientId))
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            foreach (var resource in Config.IdentityResources)
            {
                if (!context.IdentityResources.Any(r => r.Name == resource.Name))
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }

            foreach (var scope in Config.ApiScopes)
            {
                if (!context.ApiScopes.Any(s => s.Name == scope.Name))
                {
                    context.ApiScopes.Add(scope.ToEntity());
                }
            }

            foreach (var resource in Config.ApiResources)
            {
                if (!context.ApiResources.Any(r => r.Name == resource.Name))
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
            }

            context.SaveChanges();
        }
    }
}