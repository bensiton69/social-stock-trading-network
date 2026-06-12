var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var db = postgres.AddDatabase("socialstocks");

builder.AddProject<Projects.SocialStockTradingNetwork_Api>("api")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
