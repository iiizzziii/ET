using ET.Api.Data;
using ET.Api.Models;
using FluentValidation;

namespace ET.Api.Extensions;

public class PositionValidator : AbstractValidator<PositionCollection>
{
    public PositionValidator(AppDbContext dbContext)
    {
        
    }
}