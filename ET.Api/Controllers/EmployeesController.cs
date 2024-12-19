using ET.Api.Data;
using ET.Api.Extensions;
using ET.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(
        int id,
        [FromBody] EmployeeDto body)
    {
        var positions = await dbContext.Positions.ToListAsync();
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        employee.Name = body.Name;
        employee.Surname = body.Surname;
        employee.BirthDate = body.BirthDate;
        employee.IpAddress = body.IpAddress ?? employee.IpAddress;
        employee.Position = positions.FirstOrDefault(p => 
            p.PositionName == body.Position) ?? employee.Position;

        try
        {
            await dbContext.SaveChangesAsync();
            return Ok(employee);
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine(e);
            // throw;
            return StatusCode(500, "failed to update the db");
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<EmployeeDto>> AddEmployees(
        [FromBody] EmployeesCollection employees)
    {
        IEnumerable<Employee> newEmployees;
        var positions = await dbContext.Positions.ToListAsync();

        using (var httpClient = new HttpClient())
        {
            newEmployees = employees.Employees.Select(e => new Employee
            {
                //ToDo: name+surname+dob db check, cleanup, crud, error handle, serilog
                Name = e.Name,
                Surname = e.Surname,
                BirthDate = e.BirthDate,
                Position = positions.FirstOrDefault(p => p.PositionName == e.Position) ?? null,
                IpAddress = System.Net.IPAddress.TryParse(e.IpAddress, out _) ? e.IpAddress : default,
                IpCountryCode = GetIpCountyCode(e.IpAddress, httpClient).Result,
            });
            
            await dbContext.Employees.AddRangeAsync(newEmployees);
            await dbContext.SaveChangesAsync();
        }
        
        return Ok();
    }

    private static async Task<string> GetIpCountyCode(string ipAddress, HttpClient client)
    {
        var response = await client.GetAsync($"http://ip-api.com/json/{ipAddress}");
        if (!response.IsSuccessStatusCode) return "NA";
        var content = await response.Content.ReadAsStringAsync();
        var ip = JsonSerializer.Deserialize<Ip>(content);

        return ip?.CountryCode ?? "NA";
    }

    private class Ip
    {
        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; init; }
    }
}