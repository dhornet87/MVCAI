var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServerContainer("DbServer", "Start12345", 1433).AddDatabase("SQLServer");
builder.AddProject<Projects.MVCAI>("mvcai")
    .WithReference(sql);
builder.Build().Run();
