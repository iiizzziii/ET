using ET.Api.Data;
using ET.Api.Models;
using ET.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using ILogger = Serilog.ILogger;

namespace ET.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(
    ILogger logger,
    IIpService ipService,
    AppDbContext dbContext) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await dbContext.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null) return NotFound($"employee id: {id} not found");
            
        return new EmployeeDto
        {
            Name = employee.Name,
            Surname = employee.Surname,
            BirthDate = employee.BirthDate,
            Position = employee.Position?.PositionName ?? "not specified",
            IpAddress = employee.IpAddress,
        };
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        return await dbContext.Employees
            .Include(e => e.Position)
            .Select(e => new EmployeeDto {
                Name = e.Name,
                Surname = e.Surname,
                BirthDate = e.BirthDate,
                Position = e.Position!.PositionName,
                IpAddress = e.IpAddress }).ToListAsync();
    }

    [HttpGet("ids")]
    public async Task<ActionResult<int[]>> GetEmployeeIds()
    {
        return await dbContext.Employees
            .Select(e => e.EmployeeId)
            .ToArrayAsync();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(
        int id,
        [FromBody] EmployeeDto update)
    {
        var employee = await dbContext.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null) return NotFound($"employee id: {id} not found");

        var position = await dbContext.Positions
            .FirstOrDefaultAsync(p => p.PositionName == update.Position);

        employee.Name = update.Name;
        employee.Surname = update.Surname;
        employee.BirthDate = update.BirthDate;
        employee.IpAddress = update.IpAddress;
        employee.Position = position ?? employee.Position;

        if (employee.IpAddress != update.IpAddress)
        {
            employee.IpCountryCode = await ipService.GetIpCountryCode(update.IpAddress);
        }
        
        try
        {
            await dbContext.SaveChangesAsync();
            return Ok(employee);
        }
        catch (DbUpdateException e)
        {
            var m = $"failed to update employee, id: {id}";
            logger.Error(e, m);
            return StatusCode(500, m);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<string>> DeleteEmployee(int id)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        dbContext.Employees.Remove(employee);

        try
        {
            await dbContext.SaveChangesAsync();
            return Ok($"employee deleted, id: {id}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
            return StatusCode(500, "failed to remove");
        }
    }

    [HttpPost("json/positions")]
    public async Task<ActionResult<IEnumerable<Position>>> AddPositions(
        [FromBody] PositionNamesDto newNames)
    {
        var existingPositionNames = await dbContext.Positions
            .Select(p => p.PositionName)
            .ToListAsync();

        var newPositions = newNames.Positions.Where(nP => 
                !string.IsNullOrWhiteSpace(nP) &&
                !existingPositionNames.Contains(nP, StringComparer.OrdinalIgnoreCase))
            .Select(nP => new Position { PositionName = nP }).ToList();
            
        await dbContext.Positions.AddRangeAsync(newPositions);
        await dbContext.SaveChangesAsync();

        return Ok(newPositions);
    }
    
    [HttpPost]
    [Route("json")]
    public async Task<IActionResult> AddEmployees(
        [FromBody] EmployeesCollection employees)
    {
        var positions = await dbContext.Positions.ToListAsync();

        try
        {
            var newEmployees = employees.Employees.Select(e => 
                new Employee 
                {
                    Name = e.Name, 
                    Surname = e.Surname,
                    BirthDate = e.BirthDate, 
                    Position = positions.FirstOrDefault(p => p.PositionName == e.Position) ?? null,
                    IpAddress = e.IpAddress,
                    IpCountryCode = ipService.GetIpCountryCode(e.IpAddress).Result 
                });
        
            await dbContext.Employees.AddRangeAsync(newEmployees);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            logger.Error(e, "failed to add employees to db");
            throw;
        }
    }
}