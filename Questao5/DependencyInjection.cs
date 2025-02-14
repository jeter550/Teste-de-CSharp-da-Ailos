using MediatR;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Questao5
{
    public static class DependencyInjection
    {
        public static void Add(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Habilitar suporte para anotação de documentação no código
                options.EnableAnnotations();

                // Configurações adicionais para Swagger, se necessário
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });
        }

        public static void Configure(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
