using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ParkLink.Identity.Configurations
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResources.OpenId(),
                new IdentityResources.Profile
                {
                    UserClaims = { "role" }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
                new ApiResource("notificationapi", "Notification System API")
                {
                    ApiSecrets =
                    {
                        new Secret("P@rY&l**NkX".Sha256())
                    }
                },
                new ApiResource("parkingapi", "Parking System API")
                {
                    ApiSecrets =
                    {
                        new Secret("P@rY&l**NkX".Sha256())
                    }
                },
                new ApiResource("payment", "Payment System API")
                {
                    ApiSecrets =
                    {
                        new Secret("P@rY&l**NkX".Sha256())
                    }
                },
                new ApiResource("reservationapi", "Reservation System API")
                {
                    ApiSecrets =
                    {
                        new Secret("P@rY&l**NkX".Sha256())
                    }
                },
                new ApiResource("userapi", "User System API")
                {
                    ApiSecrets =
                    {
                        new Secret("P@rY&l**NkX".Sha256())
                    }
                },
                new ApiResource("vehicleapi", "Vehicle System API")
                {
                    ApiSecrets =
                    {
                        new Secret("P@rY&l**NkX".Sha256())
                    }
                }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
                new ApiScope("notificationapi", "Notification System API"),
                new ApiScope("parkingapi", "Parking System API"),
                new ApiScope("paymentapi", "Payment System API"),
                new ApiScope("reservationapi", "Reservation System API"),
                new ApiScope("userapi", "User System API"),
                new ApiScope("vehicleapi", "Vehicle System API")
        };

        public static IEnumerable<Client> Clients(IConfiguration configuration) =>
            new Client[]
            {
                new Client
                {
                    ClientId = "notification.api.code",
                    ClientName = "Notification_Api_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "notificationapi",
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:NotificationServiceUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:NotificationServiceUrl"]}/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:NotificationServiceUrl"]}/signout-callback-oidc",
                        $"{configuration["ClientUrls:NotificationServiceUrl"]}/swagger/signout-callback-oidc"
                    },
                    // Set token lifetimes
				    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "parking.api.code",
                    ClientName = "Parking_Api_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "parkingapi",
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:ParkingServiceUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:ParkingServiceUrl"]}/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:ParkingServiceUrl"]}/signout-callback-oidc",
                        $"{configuration["ClientUrls:ParkingServiceUrl"]}/swagger/signout-callback-oidc"
                    },
                    // Set token lifetimes
				    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "payment.api.code",
                    ClientName = "Payment_Api_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "paymentapi",
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:PaymentServiceUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:PaymentServiceUrl"]}/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:PaymentServiceUrl"]}/signout-callback-oidc",
                        $"{configuration["ClientUrls:PaymentServiceUrl"]}/swagger/signout-callback-oidc"
                    },
                    // Set token lifetimes
				    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "reservation.api.code",
                    ClientName = "Reservation_Api_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "reservationapi",
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:ReservationServiceUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:ReservationServiceUrl"]}/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:ReservationServiceUrl"]}/signout-callback-oidc",
                        $"{configuration["ClientUrls:ReservationServiceUrl"]}/swagger/signout-callback-oidc"
                    },
                    // Set token lifetimes
				    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "user.api.code",
                    ClientName = "User_Api_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "userapi",
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:UserServiceUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:UserServiceUrl"]}/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:UserServiceUrl"]}/signout-callback-oidc",
                        $"{configuration["ClientUrls:UserServiceUrl"]}/swagger/signout-callback-oidc"
                    },
                    // Set token lifetimes
				    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "vehicle.api.code",
                    ClientName = "Vehicle_Api_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "vehicleapi",
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:VehicleServiceUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:VehicleServiceUrl"]}/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:VehicleServiceUrl"]}/signout-callback-oidc",
                        $"{configuration["ClientUrls:VehicleServiceUrl"]}/swagger/signout-callback-oidc"
                    },
                    // Set token lifetimes
				    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "admin.web.code",
                    ClientName = "Admin_Web_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "notificationapi",
                        "parkingapi",
                        "paymentapi",
                        "reservationapi",
                        "userapi",
                        "vehicleapi"
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:AdminUIUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:AdminUIUrl"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:AdminUIUrl"]}"
                    },
                    FrontChannelLogoutUri = $"{configuration["ClientUrls:AdminUIUrl"]}/signout-oidc",
                    BackChannelLogoutUri = $"{configuration["ClientUrls:AdminUIUrl"]}/signout-oidc",
                    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                },
                new Client
                {
                    ClientId = "driver.mobile.code",
                    ClientName = "Driver_Mobile_Code",
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "notificationapi",
                        "parkingapi",
                        "paymentapi",
                        "reservationapi",
                        "userapi",
                        "vehicleapi"
                    },
                    AllowedCorsOrigins = new []
                    {
                        $"{configuration["ClientUrls:DriverMauiUrl"]}"
                    },
                    RedirectUris = new []
                    {
                        $"{configuration["ClientUrls:DriverMauiUrl"]}"
                    },
                    PostLogoutRedirectUris = new []
                    {
                        $"{configuration["ClientUrls:DriverMauiUrl"]}"
                    },
                    AccessTokenLifetime = 3600, // 1 hour
				    IdentityTokenLifetime = 300, // 5 minutes
				    AbsoluteRefreshTokenLifetime = 2592000, // 30 days
				    SlidingRefreshTokenLifetime = 1296000, // 15 days
                }
            };
    }
}
