using MediatR;
using Microsoft.EntityFrameworkCore;
using SGc.Clinica.Api.Helpers;
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

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
