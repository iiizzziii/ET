using ET.Api.Data;
using ET.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ET.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
    AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        var employees = await dbContext.Employees.ToListAsync();
        var positions = await dbContext.Positions.ToListAsync();

        return (
            from e in employees
            join p in positions
                on e.PositionId equals p.PositionId
                select new Employee
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    Surname = e.Surname,
                    DateTime = e.DateTime,
                    PositionId = p.PositionId,
                    IpAddress = e.IpAddress,
                    IpCountryCode = e.IpCountryCode,
                    Position = p,
                }
        ).ToList();
    }
}