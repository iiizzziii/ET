using ET.Api.Data;
using ET.Api.Extensions;
using ET.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ET.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
    AppDbContext dbContext) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        var positions = await dbContext.Positions.ToListAsync();
            
        return new EmployeeDto
        {
            Name = employee.Name,
            Surname = employee.Surname,
            BirthDate = employee.BirthDate,
            Position = employee.Position.PositionName,
            IpAddress = employee.IpAddress,
        };
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        var employees = await dbContext.Employees.ToListAsync();
        var positions = await dbContext.Positions.ToListAsync();

        return employees.ConvertToDto(positions).ToList();
    }

    [HttpGet("ids")]
    public async Task<ActionResult<int[]>> GetEmployeeIds()
    {
        var employees = await dbContext.Employees.ToListAsync();

        return (from e in employees select e.EmployeeId).ToArray();
    }

    [HttpPost("create")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(
        [FromBody] EmployeesCollection employees)
    {
        // if (!ModelState.IsValid) return BadRequest();
        var positions = await dbContext.Positions.ToListAsync();

        var newEmployees = employees.Employees.Select(e => new Employee
        {
            Name = e.Name,
            Surname = e.Surname,
            BirthDate = e.BirthDate,
            Position = positions.FirstOrDefault(p => p.PositionName == e.Position)
                       ?? 
                    new Position { /*PositionId = new Random().Next(),*/ PositionName = e.Position, },
            IpAddress = e.IpAddress,
            IpCountryCode = "123.456.7890",
        });

        foreach (var e in newEmployees)
        {
            await dbContext.Employees.AddAsync(e);
        }

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}