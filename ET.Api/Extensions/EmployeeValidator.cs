using ET.Api.Data;
using ET.Models;
using FluentValidation;
using System.Globalization;
using ILogger = Serilog.ILogger;

namespace ET.Api.Extensions;

// ReSharper disable once ClassNeverInstantiated.Global
public class EmployeeValidator : AbstractValidator<EmployeeDto>
{
    private const string EmptyMessage = " cannot be empty";

    public EmployeeValidator(AppDbContext dbContext, ILogger logger)
    {
        RuleFor(e => new { e.Name, e.Surname, e.BirthDate }).Custom((employee, context) =>
        {
            var exists = dbContext.Employees.Any(e =>
                e.Name == employee.Name &&
                e.Surname == employee.Surname &&
                e.BirthDate == employee.BirthDate);

            if (!exists) return;
            
            var m = $"employee {employee.Name} {employee.Surname} already exists";
            logger.Error(m);
            context.AddFailure(nameof(employee), m);
        });

        RuleFor(e => e.Name).Custom((name, context) =>
        {
            if (!string.IsNullOrWhiteSpace(name)) return;
            
            const string m = $"{nameof(name)}{EmptyMessage}";
            logger.Error(m);
            context.AddFailure(nameof(name), m);
        });
        
        RuleFor(e => e.Surname).Custom((surname, context) =>
        {
            if (!string.IsNullOrWhiteSpace(surname)) return;
            
            const string m = $"{nameof(surname)}{EmptyMessage}";
            logger.Error(m);
            context.AddFailure(nameof(surname), m);
        });
        
        RuleFor(e => e.BirthDate).Custom((birthdate, context) =>
        {
            if (!string.IsNullOrWhiteSpace(birthdate)) return;
        
            var m = $"{birthdate}{EmptyMessage}";
            logger.Error(m);
            context.AddFailure(nameof(birthdate), m);
        });
            
        RuleFor(e => e.BirthDate).Custom((birthdate, context) =>
        {
            if (DateTime.TryParseExact(
                birthdate,
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var validFormatDate) && 
                validFormatDate < DateTime.Today) return;

            var m = $"{birthdate} is not a valid date in yyyy/mm/dd format";
            logger.Error(m);
            context.AddFailure(nameof(birthdate), m);
        });    
            
        RuleFor(e => e.IpAddress).Custom((ip, context) =>
        {
            if (!string.IsNullOrWhiteSpace(ip)) return;
        
            var m = $"{ip}{EmptyMessage}";
            logger.Error(m);
            context.AddFailure(nameof(ip), m);
        });

        RuleFor(e => e.IpAddress).Custom((ip, context) =>
        {
            if (System.Net.IPAddress.TryParse(ip, out _)) return;

            var m = $"{ip} address{EmptyMessage}";
            logger.Error(m);
            context.AddFailure(nameof(ip), m);
        });
    }

    //ReSharper disable once ClassNeverInstantiated.Global
    public class EmployeeCollectionValidator : AbstractValidator<EmployeesDto> 
    {
        public EmployeeCollectionValidator(IValidator<EmployeeDto> employeeValidator) 
        {
            RuleForEach(e => e.Employees).SetValidator(employeeValidator);
            
            // RuleFor(eC => eC.Employees).ForEach(e => { e.SetValidator(employeeValidator); });
        } 
    }
}