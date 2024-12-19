using ET.Api.Data;
using ET.Api.Models;
using ET.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ET.Api.Controllers;

[ApiController]
[Route(("api/[controller]"))]
public class BulkAddController(
    AppDbContext dbContext, 
    IIpService ipService,
    HttpClient httpClient
    ) : ControllerBase
{
    
    
    [HttpPost]
    public async Task<IActionResult> AddEmployees(
        [FromBody] EmployeesCollection employees)
    {
        var positions = await dbContext.Positions.ToListAsync();

        using (var httpClient = new HttpClient())
        {
            var newEmployees = employees.Employees.Select(e => 
                new Employee {
                    Name = e.Name,
                    Surname = e.Surname,
                    BirthDate = e.BirthDate,
                    Position = positions.FirstOrDefault(p => p.PositionName == e.Position) ?? null,
                    IpAddress = System.Net.IPAddress.TryParse(e.IpAddress, out _) ? e.IpAddress : default,
                    IpCountryCode = ipService.GetIpCountryCode(e.IpAddress!).Result
                });
            await dbContext.Employees.AddRangeAsync(newEmployees);
            await dbContext.SaveChangesAsync();
        }
        return Ok();
    }
}