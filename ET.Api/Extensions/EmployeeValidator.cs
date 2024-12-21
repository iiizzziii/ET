using ET.Api.Data;
using ET.Api.Models;
using FluentValidation;
using System.Globalization;

namespace ET.Api.Extensions;

public class EmployeeValidator : AbstractValidator<EmployeeDto>
{
    public EmployeeValidator(AppDbContext dbContext)
    {
        RuleFor(e => new { e.Name, e.Surname, e.BirthDate })
            .Custom((employee, context) =>
            {
                var exists = dbContext.Employees.Any(e =>
                    e.Name == employee.Name &&
                    e.Surname == employee.Surname &&
                    e.BirthDate == employee.BirthDate);

                if (exists)
                {
                    context.AddFailure(nameof(employee), "employee with equal name/surname/birthdate already exists");
                }
            });
        
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("name cannot be empty");
        
        RuleFor(e => e.Surname)
            .NotEmpty().WithMessage("surname cannot be empty");
        
        RuleFor(e => e.BirthDate)
            .NotEmpty().WithMessage("birthdate cannot be empty")
            .Must(ValidDate).WithMessage("birthdate format not valid");
        
        RuleFor(e => e.Position)
            .NotEmpty().WithMessage("position cannot be empty");
        
        RuleFor(e => e.IpAddress)
            .NotEmpty().WithMessage("ip address cannot be empty")
            .Must(ValidIpAddress!).WithMessage("ip address not valid");
    }

    public class EmployeeCollectionValidator : AbstractValidator<EmployeesCollection> 
    {
        public EmployeeCollectionValidator(AppDbContext dbContext) 
        {
            RuleFor(eC => eC.Employees).ForEach(e => 
            {
                e.SetValidator(new EmployeeValidator(dbContext)); 
            }); 
        } 
    }
    
    private static bool ValidDate(string inputDate) =>

        DateTime.TryParseExact(
            inputDate, 
            "yyyy/MM/dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var validFormatDate) 
        && validFormatDate < DateTime.Today;
    
    private bool ValidIpAddress(string ip) => System.Net.IPAddress.TryParse(ip, out _);
}