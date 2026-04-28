using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MusicStore
{
    public class MusicStoreContextFactory : IDesignTimeDbContextFactory<MusicStoreContext>
    {
        public MusicStoreContext CreateDbContext(string[] args)
        {
            // Ruta absoluta segura, incluso con espacios
            var projectDir = AppContext.BaseDirectory;

            // Retrocede hasta la raíz del proyecto (busca MusicStore.csproj)
            while (projectDir != null && !File.Exists(Path.Combine(projectDir, "MusicStore.csproj")))
                projectDir = Directory.GetParent(projectDir)?.FullName;

            if (projectDir == null)
                throw new DirectoryNotFoundException("No se encontró la raíz del proyecto MusicStore.");

            var appSettingsPath = Path.Combine(projectDir, "appsettings.json");

            if (!File.Exists(appSettingsPath))
                throw new FileNotFoundException($"No se encontró appsettings.json en: {appSettingsPath}");

            // Carga configuración desde el archivo
            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var optionsBuilder = new DbContextOptionsBuilder<MusicStoreContext>();
            optionsBuilder.UseNpgsql(connectionString);

            Console.WriteLine($"✅ Cargando configuración desde: {appSettingsPath}");
            Console.WriteLine($"🔗 Conexión: {connectionString}");

            return new MusicStoreContext(optionsBuilder.Options);
        }
    }
}
