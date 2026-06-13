// Dashboard login is disabled via ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS
// in Properties/launchSettings.json (and aspire.config.json profiles).
// Setting Dashboard:AuthMode in code or appsettings.json is overridden by
// the AppHost at startup and does not disable the login prompt.
var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", "postgres", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("socialstocks");

builder.AddProject<Projects.SocialStockTradingNetwork_Api>("api")
    .WithReference(db, connectionName: "Default")
    .WaitFor(db);

builder.Build().Run();
