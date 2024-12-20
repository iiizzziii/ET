using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using ET.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ET.Api.Models;

#pragma warning disable CS8618

[Index(nameof(Name), nameof(Surname), nameof(BirthDate), IsUnique = true)]
public class Employee : IValidatableObject
{
    private string _birthDate;
    
    [Key]
    public int EmployeeId { get; init; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Surname { get; set; }
    
    [Required]
    public string BirthDate 
    { 
        get => _birthDate;
        set
        {
            ValidDateFormat(value);
            _birthDate = value;
        }
    }
    
    [Required]
    public string? IpAddress { get; set; }
    
    public string? IpCountryCode { get; set; }
    
    [ForeignKey(nameof(Position))]
    public int? PositionId { get; set; }
    
    public Position? Position { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is AppDbContext dbContext && 
            dbContext.Employees.Any(e => 
                e.Name == Name && 
                e.Surname == Surname && 
                e.BirthDate == BirthDate))
        {
            yield return new ValidationResult("employee with equal Name/Surname/BirthDate already exists");
        }
    }

    private static void ValidDateFormat(string inputDate)
    {
        if (DateTime.TryParseExact(
                inputDate,
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var validFormatDate) && validFormatDate < DateTime.Today)
        {
            return;
        }

        throw new ArgumentException("birthdate not in correct format");
    }
}