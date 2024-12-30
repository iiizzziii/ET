using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ET.Models;

public class EmployeeForm : IValidatableObject
{
    public int Id { get; init; }

    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Surname is required.")]
    public required string Surname { get; set; }

    [Required(ErrorMessage = "Birth Date is required.")]
    public required string BirthDate { get; set; }

    [Required(ErrorMessage = "Position is required.")]
    public required string Position { get; set; }

    [Required(ErrorMessage = "IP Address is required.")]
    public required string IpAddress { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!DateTime.TryParseExact(
                BirthDate,
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var validFormatDate) &&
            validFormatDate < DateTime.Today)
        {
            yield return new ValidationResult(
                "Birth date not a valid date in YYYY/MM/DD format",
                [nameof(BirthDate)]);
        }

        if (!System.Net.IPAddress.TryParse(IpAddress, out _))
        {
            yield return new ValidationResult(
                "Ip address not valid",
                [nameof(IpAddress)]);
        }
    }
}