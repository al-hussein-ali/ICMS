using FluentValidation;
using ICMS.Application.Validators;
using ICMS.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");



builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<AssemblyMarker>(includeInternalTypes:true); // fluent validation registration

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("ICMS API");
        options.WithTheme(ScalarTheme.Default);
        options.WithDefaultHttpClient(ScalarTarget.CSharp,ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
