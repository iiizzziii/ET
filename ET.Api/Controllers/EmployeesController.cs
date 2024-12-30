using ET.Api.Data;
using ET.Api.Services;
using ET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace ET.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
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
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await dbContext.Employees
            .Include(e => e.Position)
            .ToListAsync();
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateEmployee(
        int id,
        [FromBody] EmployeeDto update)
    {
        try
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
            
            await dbContext.SaveChangesAsync();
            return Ok($"employee {employee.Name} {employee.Surname} updated");
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound($"employee id: {id} not found");
        }
        catch (Exception e)
        {
            var m = $"failed to update employee, id: {id}";
            logger.Error(e, m);
            return StatusCode(500, m);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        try
        {
            var employee = new Employee { EmployeeId = id };
            dbContext.Employees.Attach(employee);
            dbContext.Employees.Remove(employee);
            
            await dbContext.SaveChangesAsync();
            return Ok($"employee deleted, id: {id}");
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound($"employee id: {id} not found");
        }
        catch (Exception e)
        {
            var m = $"failed to delete employee, id: {id}";
            logger.Error(e, m);
            return StatusCode(500, m);
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddEmployee(
        [FromBody] EmployeeDto employee)
    {
        try
        {
            var positionMatch = await dbContext.Positions
                .Where(p => p.PositionName.Equals(employee.Position))
                .FirstOrDefaultAsync();
        
            var newEmployee = new Employee
            {
                Name = employee.Name,
                Surname = employee.Surname,
                BirthDate = employee.BirthDate,
                IpAddress = employee.IpAddress,
                IpCountryCode = await ipService.GetIpCountryCode(employee.IpAddress),
                Position = positionMatch ?? null,
            };
        
            await dbContext.Employees.AddAsync(newEmployee);
            await dbContext.SaveChangesAsync();
        
            return Ok($"employee {newEmployee.Name} {newEmployee.Surname} added");
        }
        catch (Exception e)
        {
            const string m = "failed to add a new employee";
            logger.Error(e, m);
            return StatusCode(500, m);
        }
    }
    
    [HttpPost]
    [Route("add/json")]
    public async Task<IActionResult> AddEmployees(
        [FromBody] EmployeesDto employees)
    {
        try
        {
            var positionNames = employees.Employees
                .Select(e => e.Position)
                .Distinct()
                .ToList();

            var positionMatch = await dbContext.Positions
                .Where(p => positionNames.Contains(p.PositionName))
                .ToDictionaryAsync(p => p.PositionName, p => p);
            
            var newEmployees = employees.Employees.Select(async e => 
                new Employee 
                {
                    Name = e.Name, 
                    Surname = e.Surname,
                    BirthDate = e.BirthDate, 
                    IpAddress = e.IpAddress,
                    IpCountryCode = await ipService.GetIpCountryCode(e.IpAddress),
                    Position = positionMatch.GetValueOrDefault(e.Position) ?? null,
                }).ToArray();
            
            await dbContext.Employees.AddRangeAsync(await Task.WhenAll(newEmployees));
            await dbContext.SaveChangesAsync();

            return Ok($"{newEmployees.Length} new employees added");
        }
        catch (Exception e)
        {
            const string m = "failed to add new employees";
            logger.Error(e, m);
            return StatusCode(500, m);
        }
    }
    
    [HttpPost("add/json/positions")]
    public async Task<ActionResult> AddPositions(
        [FromBody] PositionsDto positions)
    {
        var incomingPositions = positions.Positions
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (incomingPositions.Count.Equals(0)) return BadRequest("no valid positions to add");

        try
        {
            var existingPositions = await dbContext.Positions
                .Where(p => incomingPositions.Contains(p.PositionName))
                .Select(p => p.PositionName)
                .ToListAsync();

            var newPositions = incomingPositions
                .Except(existingPositions, StringComparer.OrdinalIgnoreCase)
                .Select(p => new Position { PositionName = p })
                .ToList();

            if (newPositions.Count.Equals(0)) return Ok("no new positions to add");
            
            await dbContext.Positions.AddRangeAsync(newPositions);
            await dbContext.SaveChangesAsync();

            return Ok($"added {newPositions.Count} position(s)");
        }
        catch (Exception e)
        {
            const string m = "failed to add new positions";
            logger.Error(e, m);
            return StatusCode(500, m);
        }
    }
    
    [HttpGet("ids")]
    public async Task<ActionResult<int[]>> GetEmployeeIds()
    {
        return await dbContext.Employees
            .Select(e => e.EmployeeId)
            .OrderBy(id => id)
            .ToArrayAsync();
    }

    [HttpGet("positions")]
    public async Task<ActionResult<IEnumerable<string>>> GetPositions()
    {
        return await dbContext.Positions
            .Select(p => p.PositionName)
            .ToListAsync();
    }
}