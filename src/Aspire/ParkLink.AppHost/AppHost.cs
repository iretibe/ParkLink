var builder = DistributedApplication.CreateBuilder(args);

// Add services to the container.

// SQL Server
var sqlPassword = builder.AddParameter("SqlPassword", true);
var sqlServer = builder.AddSqlServer("sqlserver", password: sqlPassword)
    .WithHostPort(14333)
    .WithDataVolume("parklink-sqldata")
    .WithLifetime(ContainerLifetime.Persistent); // Keeps running after 'dotnet run' stops

var identityDb = sqlServer.AddDatabase("parklink-identitydb");
var parkingDb = sqlServer.AddDatabase("parklink-parkingdb");
var reservationDb = sqlServer.AddDatabase("parklink-reservationdb");
var paymentDb = sqlServer.AddDatabase("parklink-paymentdb");
var notificationDb = sqlServer.AddDatabase("parklink-notificationdb");
var vehicleDb = sqlServer.AddDatabase("parklink-vehicledb");
var gateDb = sqlServer.AddDatabase("parklink-gatedb");

// Redis
var redis = builder.AddRedis("redis")
    .WithRedisCommander() // Adds a UI at http://localhost:19000 to inspect keys
    .WithLifetime(ContainerLifetime.Persistent);

// RabbitMQ
var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin() // Adds the RabbitMQ Management UI (Port 15672)
    .WithLifetime(ContainerLifetime.Persistent);

// Seq (Centralized Logging & Structured Log Viewer)
// Perfect for inspecting logs from all microservices in one place.
var seq = builder.AddSeq("seq")
    .WithDataVolume("parklink-seqdata")
    .WithLifetime(ContainerLifetime.Persistent);

// MailPit (Email Testing Tool)
//var mailpit = builder.AddMailpit("mailpit")
//    .WithLifetime(ContainerLifetime.Persistent);

// Services
var identityService = builder
    .AddProject<Projects.ParkLink_Identity>("parking-identity")
    .WithEndpoint(
        endpointName: "https",
        endpoint =>
        {
            endpoint.Port = 5001;
            endpoint.TargetPort = 5001;
            endpoint.UriScheme = "https";
            endpoint.IsProxied = false; // Prevents Aspire proxy from holding port 5001
        })
    .WithUrl("https://parking-identity-parklink.127.0.0.1.nip.io:5001")
    .WithReference(identityDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(identityDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var notificationService = builder
    .AddProject<Projects.ParkLink_Notification>("parking-notification")
    .WithUrl("https://parking-notification-api-parklink.127.0.0.1.nip.io:7090")
    .WithReference(notificationDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(notificationDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/alive")
    //.WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var parkingService = builder
    .AddProject<Projects.ParkLink_Parking>("parking-parking")
    .WithUrl("https://parking-parkng-api-parklink.127.0.0.1.nip.io:7009")
    .WithReference(parkingDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(parkingDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/alive")
    //.WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var paymentService = builder
    .AddProject<Projects.ParkLink_Payment>("parking-payment")
    .WithUrl("https://parking-payment-api-parklink.127.0.0.1.nip.io:7030")
    .WithReference(paymentDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(paymentDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/alive")
    //.WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var reservationService = builder
    .AddProject<Projects.ParkLink_Reservation>("parking-reservation")
    .WithUrl("https://parking-reservation-api-parklink.127.0.0.1.nip.io:7064")
    .WithReference(reservationDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(reservationDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/alive")
    //.WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var userService = builder
    .AddProject<Projects.ParkLink_Users>("parking-users")
    .WithUrl("https://parking-user-api-parklink.127.0.0.1.nip.io:7269")
    .WithReference(identityDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(identityDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var vehicleService = builder
    .AddProject<Projects.ParkLink_Vehicle>("parking-vehicle")
    .WithUrl("https://parking-vehicle-api-parklink.127.0.0.1.nip.io:7066")
    .WithReference(vehicleDb)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(vehicleDb)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var gateService = builder
    .AddProject<Projects.ParkLink_Gate>("parking-gate")
    .WithReference(gateDb)
    .WithReference(identityService)
    .WithReference(vehicleService)
    .WithReference(reservationService)
    .WithReference(paymentService)
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(seq)
    .WaitFor(gateDb)
    .WaitFor(identityService)
    .WaitFor(vehicleService)
    .WaitFor(reservationService)
    .WaitFor(paymentService)
    .WaitFor(redis)
    .WaitFor(rabbitMq)
    .WaitFor(seq)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

var gatewayService = builder
    .AddProject<Projects.ParkLink_ApiGateway>("parking-gateway")
    .WithReference(userService)
    .WithReference(vehicleService)
    .WithReference(parkingService)
    .WithReference(reservationService)
    .WithReference(paymentService)
    .WithReference(notificationService)
    .WithReference(gateService)
    .WithReference(identityService)
    .WithReference(redis)
    .WaitFor(identityService)
    .WaitFor(userService)
    .WaitFor(vehicleService)
    .WaitFor(parkingService)
    .WaitFor(reservationService)
    .WaitFor(paymentService)
    .WaitFor(notificationService)
    .WaitFor(gateService)
    .WaitFor(redis)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
