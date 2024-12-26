using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ET.Api.Models;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Position
{
    [Key]
    public int PositionId { get; set; }
    public string PositionName { get; set; }
    public ICollection<Employee> Employees { get; set; }
}