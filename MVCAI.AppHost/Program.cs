var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MVCAI>("mvcai");

builder.Build().Run();
