using DocumentDb;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<DocumentDbContext>("DocumentDb");

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DocumentDbInitializer.ActivitySourceName));

builder.Services.AddSingleton<DocumentDbInitializer>();

builder.Services.AddHostedService(sp => sp.GetRequiredService<DocumentDbInitializer>());
builder.Services.AddHealthChecks().AddCheck<DocumentDbInitializerHealthCheck>("DbInitializer", null);
var app = builder.Build();

app.MapDefaultEndpoints();

await app.RunAsync();

