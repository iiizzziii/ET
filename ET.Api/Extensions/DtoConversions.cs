using ET.Api.Models;

namespace ET.Api.Extensions;

public static class DtoConversions
{
    public static IEnumerable<EmployeeDto> ConvertToDto(
        this IEnumerable<Employee> employees,
        IEnumerable<Position> positions)
    {
        return 
            from e in employees
            join p in positions on e.PositionId equals p.PositionId
            select new EmployeeDto
            {
                Name = e.Name,
                Surname = e.Surname,
                BirthDate = e.BirthDate,
                Position = p.PositionName,
                IpAddress = e.IpAddress
            };
    }
}