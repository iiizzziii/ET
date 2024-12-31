using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ET.Models;

public class EmployeeForm : IValidatableObject
{
    public int Id { get; init; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required.")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Birth Date is required.")]
    public string BirthDate { get; set; }

    [Required(ErrorMessage = "Position is required.")]
    public string Position { get; set; }

    [Required(ErrorMessage = "IP Address is required.")]
    public string IpAddress { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!DateTime.TryParseExact(
                BirthDate,
                "yyyy/MM/dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dateTime))
        {
            yield return new ValidationResult(
                "Birth date not in YYYY/MM/DD format",
                [nameof(BirthDate)]);
        }

        if (dateTime > DateTime.Now)
        {
            yield return new ValidationResult(
                "Birth date must be in past",
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