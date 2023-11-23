var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServerContainer("DbServer", "NetHackathon8", 1433).AddDatabase("DocumentDb");
builder.AddProject<Projects.MVCAI>("mvcai")
    .WithReference(sql);


//builder.AddProject<Projects.DocumentService>("documentservice");

builder.AddProject<Projects.DocumentDb>("documentdb")
    .WithReference(sql);

builder.Build().Run();
