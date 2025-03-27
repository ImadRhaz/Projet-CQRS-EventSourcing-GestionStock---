using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GestionFM1.Write.EventStore
{
    public class EventStoreDbContextFactory : IDesignTimeDbContextFactory<EventStoreDbContext>
    {
        public EventStoreDbContext CreateDbContext(string[] args)
        {
            // Afficher le répertoire de travail actuel pour déboguer
            Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());

            // Construire le chemin vers le fichier appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\GestionFM1.API");
            Console.WriteLine("Base Path: " + basePath);

            // Charger la configuration depuis appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            // Afficher la chaîne de connexion pour déboguer
            var connectionString = configuration.GetConnectionString("EventStoreConnection");
            Console.WriteLine("Connection String: " + connectionString);

            // Configurer DbContextOptions
            var builder = new DbContextOptionsBuilder<EventStoreDbContext>();
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("GestionFM1.Write"));

            // Retourner une nouvelle instance de EventStoreDbContext
            return new EventStoreDbContext(builder.Options);
        }
    }
}