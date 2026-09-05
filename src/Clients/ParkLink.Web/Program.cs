using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using MudBlazor;
using MudBlazor.Services;
using ParkLink.Shared.Clients;
using ParkLink.Shared.Providers;
using ParkLink.Shared.Security;
using ParkLink.Shared.Services;
using ParkLink.Web.Components;
using ParkLink.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddHubOptions(o =>
    {
        o.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
        o.HandshakeTimeout = TimeSpan.FromMinutes(2);
        o.KeepAliveInterval = TimeSpan.FromSeconds(15);
    })
    .AddCircuitOptions(o =>
    {
        o.DetailedErrors = true;
        o.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
        o.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(5);
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies", options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = builder.Configuration["IdentityOidc:AuthorityUrl"];
    options.ClientId = builder.Configuration["IdentityOidc:ClientId"];
    options.ResponseType = builder.Configuration["IdentityOidc:ResponseType"]!;

    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    //options.SignedOutCallbackPath = builder.Configuration["IdentityOidc:SignedOutRedirectUri"];
    options.SignedOutCallbackPath = "/signout-callback-oidc";

    // Explicit scheme wiring
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.SaveTokens = true;
    options.UsePkce = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    // Claim mappings
    options.TokenValidationParameters.NameClaimType = "name";
    options.TokenValidationParameters.RoleClaimType = "role";

    // Scopes
    foreach (var scope in builder.Configuration
        .GetSection("IdentityOidc:Scopes")
        .Get<string[]>()!)
    {
        options.Scope.Add(scope);
    }

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = ctx =>
        {
            var identity = (System.Security.Claims.ClaimsIdentity)ctx.Principal?.Identity!;

            var token = ctx.TokenEndpointResponse?.AccessToken;
            if (!string.IsNullOrEmpty(token))
            {
                identity.AddClaim(new System.Security.Claims.Claim("access_token", token));
            }

            return Task.CompletedTask;
        },
        OnSignedOutCallbackRedirect = context =>
        {
            context.Response.Redirect("/login");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRemoteFailure = ctx =>
        {
            ctx.Response.Redirect("/Account/Login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        }
    };

    if (builder.Environment.IsDevelopment())
    {
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        IdentityModelEventSource.ShowPII = true;
    }
});

// Global AUth (this replaces all UI redirects)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpClient();
builder.Services.AddHostedService<OidcWarmupService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiClient>();
//builder.Services.AddScoped<AuthApiClient>();
//builder.Services.AddScoped<ApprovalApiClient>();
//builder.Services.AddScoped<LookupApiClient>();
////builder.Services.AddScoped<ApprovalWorkflowApiClient>();
//builder.Services.AddScoped<RequisitionApiClient>();
//builder.Services.AddScoped<SupplierApiClient>();
//builder.Services.AddScoped<PurchaseApiClient>();
//builder.Services.AddScoped<DashboardApiClient>();
//builder.Services.AddScoped<GoodReceiptApiClient>();
////builder.Services.AddScoped<MenuState>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.PopoverOptions.CheckForPopoverProvider = true;
});

//builder.Services.AddAuthorization(options =>
//{
//    var permissions = new[]
//    {
//        "REQ.VIEW","REQ.CREATE","REQ.APPROVE",
//        "PO.VIEW","PO.CREATE",
//        "REC.VIEW", "REC.CREATE",
//        "INV.VIEW","INV.MANAGE",
//        "SUP.VIEW","SUP.CREATE","SUP.EDIT","SUP.DELETE",
//        "DOC.VIEW","DOC.UPLOAD",
//        "RPT.VIEW","RPT.SPEND","RPT.INV","RPT.SUP",
//        "ADM.MANAGE","ADM.USERS","ADM.ROLES","ADM.LIMITS", "ADM.DELEGATIONS", "ADM.ROUTINGS",
//        "WF.INBOX", "WF.APPROVE", "WF.DELEGATE"
//    };

//    foreach (var p in permissions)
//    {
//        options.AddPolicy(p, policy =>
//            policy.Requirements.Add(new PermissionRequirement(p)));
//    }
//});

//builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
//builder.Services.AddScoped<DashboardSignalRClient>();

// Add device-specific services used by the ParkLink.Web.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    //app.Use(async (ctx, next) =>
    //{
    //    ctx.Response.Headers["Content-Security-Policy"] =
    //        "default-src 'self'; " +
    //        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
    //        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
    //        "font-src 'self' data: https://fonts.gstatic.com; " +
    //        "img-src 'self' data:; " +
    //        "connect-src 'self' wss://mawums_web.dev.localhost:7140;";

    //    await next();
    //});
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

//app.Use(async (ctx, next) =>
//{
//    ctx.Response.Headers["Content-Security-Policy"] =
//        "default-src 'self'; " +
//        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
//        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
//        "font-src 'self' data: https://fonts.gstatic.com; " +
//        "img-src 'self' data:; " +
//        "connect-src 'self' " +
//            "https://localhost:* " +
//            "http://localhost:* " +
//            "wss://localhost:* " +
//            "ws://localhost:*;";

//    await next();
//});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", async (HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/";

    await context.ChallengeAsync("oidc", new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
});

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(ParkLink.Shared._Imports).Assembly);

app.Run();
