using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET.Api.Models;

#pragma warning disable CS8618

public class Employee
{
    [Key]
    public int EmployeeId { get; init; }
    
    [ForeignKey(nameof(Position))]
    public int? PositionId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string BirthDate { get; set; }
    public string IpAddress { get; set; }
    public string IpCountryCode { get; set; }
    public Position? Position { get; set; }
}