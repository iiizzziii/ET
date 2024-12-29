using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ET.Models;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Employee
{
    [Key] // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public int EmployeeId { get; set; }
    
    [ForeignKey(nameof(Position))] // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public int? PositionId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string BirthDate { get; set; }
    public string IpAddress { get; set; }
    public string IpCountryCode { get; set; }
    public Position? Position { get; set; }
}