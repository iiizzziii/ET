namespace ET.Api.Models;

public class EmployeeDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Position { get; set; }
    public string IpAddress { get; set; }
}

public class EmployeesCollection
{
    public List<EmployeeDto> Employees { get; set; }
}