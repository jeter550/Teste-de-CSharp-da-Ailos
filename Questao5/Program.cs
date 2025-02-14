using FluentAssertions.Common;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Database.Sqlite;
using Questao5.Infrastructure.Repositories.Interfaces;
using Questao5.Infrastructure.Repositories.Sqlite;
using System.Data;
using System.Reflection;
using Questao5.Application;
using Questao5.Infrastructure;
using Questao5;

var builder = WebApplication.CreateBuilder(args);

builder.Add();
builder.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

app.Configure();
app.ConfigureApplication();

app.Run();


