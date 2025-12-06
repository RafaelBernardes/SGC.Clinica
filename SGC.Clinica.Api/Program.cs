using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SGc.Clinica.Api.Helpers;
using SGC.Clinica.Api.Application.Behaviors;
using SGC.Clinica.Api.Data;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Middleware;
using SGC.Clinica.Api.Repositories;
using SGC.Clinica.Api.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//repository
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// MediatR 
builder.Services.AddMediatR(typeof(Program).Assembly);

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//DbInitializer
builder.Services.AddScoped<IApplicationDbContext, AppDbContext>();

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
