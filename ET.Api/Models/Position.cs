using System.ComponentModel.DataAnnotations;

namespace ET.Api.Models;

public class Position
{
    [Key]
    public int PositionId { get; set; }
    
    [Required]
    public string PositionName { get; set; }
}