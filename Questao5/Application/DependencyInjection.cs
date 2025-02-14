using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Database.Sqlite;

namespace Questao5.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this WebApplicationBuilder builder)
        {
            // Comands
            builder.Services.AddScoped<IRequest<bool>, CriarContaCorrenteCommand>();
            builder.Services.AddScoped<IRequest<string>, CriarMovimentoCommand>();

            // Commands & Queries
            builder.Services.AddScoped<IRequest<SaldoResponse>, ConsultarSaldoQuery>();
            builder.Services.AddScoped<IRequestHandler<ConsultarSaldoQuery, SaldoResponse>, ConsultarSaldoHandler>();
            builder.Services.AddScoped<IRequestHandler<CriarMovimentoCommand, string>, CriarMovimentoHandler>();
        }

        public static void ConfigureApplication(this WebApplication app)
        {
            // sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            
// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html
        }
    }
}
