using SGC.Clinica.API.Composition;
using SGC.Clinica.API.Middleware;
using SGC.Clinica.Application;
using SGC.Clinica.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
