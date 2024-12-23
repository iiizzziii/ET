using System.ComponentModel.DataAnnotations;

namespace ET.Api.Models;

public class Position
{
    [Key]
    public int PositionId { get; set; }
    
    [Required]
    public string PositionName { get; set; }
    public ICollection<Employee> Employees { get; set; }
}

public class PositionNamesDto
{
    public List<string> Positions { get; set; }
}