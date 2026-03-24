var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume("grocery-postgres-data")
    .WithPgAdmin();

var db = postgres.AddDatabase("grocery-db");

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.GroceryPromoApi>("api")
    .WithReference(db)
    .WithReference(redis);

builder.Build().Run();
