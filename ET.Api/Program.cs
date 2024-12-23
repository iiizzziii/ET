using ET.Api.Data;
using ET.Api.Extensions;
using ET.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((_, config) => {
    config.WriteTo.File("log.txt"); });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(
        "DefaultConnection")));

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssemblyContaining<EmployeeValidator.EmployeeCollectionValidator>();

builder.Services.AddHttpClient<IIpService, IpService>();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();