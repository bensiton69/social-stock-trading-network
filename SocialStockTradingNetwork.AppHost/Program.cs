var builder = DistributedApplication.CreateBuilder(args);

// Disable the Aspire Dashboard login token for local development.
// The AppHost reads these keys and forwards them as env-vars to the
// dashboard subprocess (DASHBOARD__FRONTEND__AUTHMODE etc.).
builder.Configuration["Dashboard:Frontend:AuthMode"] = "Unsecured";
builder.Configuration["Dashboard:Otlp:AuthMode"] = "Unsecured";

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("socialstocks");

builder.AddProject<Projects.SocialStockTradingNetwork_Api>("api")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
