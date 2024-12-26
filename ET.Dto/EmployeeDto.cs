using System.Diagnostics.CodeAnalysis;

namespace ET.Dto;

#pragma warning disable CS8618

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class EmployeeDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string BirthDate { get; set; }
    public string Position { get; set; }
    public string IpAddress { get; set; }
}