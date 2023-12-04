
using DocumentDb.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace DocumentDb
{
    public class DocumentDbInitializer(IServiceProvider serviceProvider, ILogger<DocumentDbInitializer> logger) : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";

        private readonly ActivitySource _activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();

            await InitializeDb(dbContext, stoppingToken);
        }

        private async Task InitializeDb(DocumentDbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            using var activity = _activitySource.StartActivity("Migrating document database", ActivityKind.Client);

            var sw = Stopwatch.StartNew();

            await strategy.ExecuteAsync(() => dbContext.Database.EnsureCreatedAsync(cancellationToken));

            await SeedDb(dbContext, cancellationToken);

            logger.LogInformation($"Database Initialization completed after {sw.ElapsedMilliseconds} ms");
        }

        private async Task SeedDb(DocumentDbContext dbContext, CancellationToken cancellationToken)
        {
            logger.LogInformation("Seeding Database");

            static List<Maincategory> GetPreConfiguredMaincategories()
            {
                return new List<Maincategory> { 
                    new Maincategory { Name = "Rechnungen"},
                    new Maincategory { Name = "Versicherung"},
                    new Maincategory { Name = "Gehalt"},
                    new Maincategory { Name = "Werbung"},
                
                };
            }

            static List<Subcategory> GetPreConfigruedSubcategories()
            {
                return new List<Subcategory>
                {
                    new Subcategory { Name= "Sanitär"},
                    new Subcategory { Name= "Stadt"},
                    new Subcategory { Name= "Telefon"},
                    new Subcategory { Name= "Internet"},
                    new Subcategory { Name= "Müll"},
                };
            }

            if(!dbContext.Maincategories.Any())
            {
                var maincategories = GetPreConfiguredMaincategories();

                await dbContext.Maincategories.AddRangeAsync(maincategories);

                logger.LogInformation($"Seeding Maincategories: {maincategories.Count} ");

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            if (!dbContext.Subcategories.Any())
            {
                var subcategories = GetPreConfigruedSubcategories();

                await dbContext.Subcategories.AddRangeAsync(subcategories);

                logger.LogInformation($"Seeding Subcategories: {subcategories.Count} ");

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
