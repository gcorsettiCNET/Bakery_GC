var builder = DistributedApplication.CreateBuilder(args);

// Aggiungi l'API OrderService
var orderApi = builder.AddProject<Projects.Bakery_OrderService>("orderservice");

// Aggiungi il sito web Bakery_GC con riferimento all'API
builder.AddProject<Projects.Bakery_GC>("webapp")
    .WithReference(orderApi);

builder.Build().Run();
