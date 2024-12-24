using ET.Api.Data;
using ET.Api.Extensions;
using ET.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

const string logFilePath = "Log/log.txt";
if (File.Exists(logFilePath)) File.WriteAllText(logFilePath, string.Empty);

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.File(
        path:logFilePath,
        shared: true,
        rollingInterval: RollingInterval.Infinite,
        restrictedToMinimumLevel: LogEventLevel.Error);
});

builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssemblyContaining<EmployeeValidator.EmployeeCollectionValidator>();

builder.Services.AddHttpClient<IIpService, IpService>();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();