using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET.Api.Models;

public class Employee
{
    [Key]
    public int EmployeeId { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Surname { get; set; }
    
    [Required]
    public string BirthDate { get; set; }
    
    [Required]
    public string? IpAddress { get; set; }
    
    public string IpCountryCode { get; set; }
    
    [ForeignKey(nameof(Position))]
    public int? PositionId { get; set; }
    
    public Position? Position { get; set; }
}