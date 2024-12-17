using ET.Api.Models;
using FluentValidation;

namespace ET.Api.Extensions;

public class AddEmployeeValidator : AbstractValidator<EmployeeDto>
{
    public AddEmployeeValidator()
    {
        RuleFor(e => e.Name).NotEmpty().WithMessage("name cannot be empty");
        RuleFor(e => e.Surname).NotEmpty().WithMessage("surname cannot be empty");
        RuleFor(e => e.BirthDate).NotEmpty().WithMessage("birthdate cannot be empty");
        RuleFor(e => e.Position).NotEmpty().WithMessage("position cannot be empty");
        RuleFor(e => e.IpAddress).NotEmpty().WithMessage("ip address cannot be empty");
        
        //ToDo: validations: date format, ip format, name+surname+birth duplicate 
    }

    public class EmployeesCollectionValidator : AbstractValidator<EmployeesCollection>
    {
        public EmployeesCollectionValidator()
        {
            RuleFor(eC => eC.Employees).ForEach(e =>
            {
                e.SetValidator(new AddEmployeeValidator());
            });
        }
    }
}