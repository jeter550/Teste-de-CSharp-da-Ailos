using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Database.Sqlite;
using Questao5.Infrastructure.Repositories.Interfaces;
using Questao5.Infrastructure.Repositories.Sqlite;
using System.Data;

namespace Questao5.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this WebApplicationBuilder builder)
        {
            // sqlite
            builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite") });
            builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

            // Registro do IDbConnection para SQLite
            builder.Services.AddSingleton<IDbConnection>(sp =>
            {
                var config = sp.GetRequiredService<DatabaseConfig>();
                return new SqliteConnection(config.Name);
            });

            // Repositories
            builder.Services.AddScoped<ISqliteIdempotenciaRepository, SqliteIdempotenciaRepository>();
            builder.Services.AddScoped<ISqliteContaCorrenteRepository, SqliteContaCorrenteRepository>();
        }
    }
}
