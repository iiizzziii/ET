using System.Data;
using ET.Api.Models;
using FluentValidation;
using FluentValidation.Validators;

namespace ET.Api.Extensions;

public class AddEmployeeValidator : AbstractValidator<EmployeeDto>
{
    public AddEmployeeValidator()
    {
        RuleFor(e => e.Name).NotEmpty().WithMessage("name cannot be empty");
        RuleFor(e => e.Surname).NotEmpty().WithMessage("surname cannot be empty");
        RuleFor(e => e.BirthDate).NotEmpty().WithMessage("name cannot be empty");
    }
}