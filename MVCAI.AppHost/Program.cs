var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MVCAI>("mvcai");
builder.AddSqlServerContainer("DbServer", "Start12345", 1433);
builder.Build().Run();
