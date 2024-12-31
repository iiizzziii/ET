using ET.Models;
using System.Net.Http.Json;

namespace ET.Web.Services;

#pragma warning disable CS8603

public class EmployeeService(HttpClient httpClient) : IEmployeeService
{
    private const string ApiUri = "api/employees";
    
    public async Task<bool> AddEmployee(EmployeeForm employee)
    {
        try
        {
            var newEmployee = new EmployeeDto
            {
                Name = employee.Name,
                Surname = employee.Surname,
                BirthDate = employee.BirthDate,
                Position = employee.Position,
                IpAddress = employee.IpAddress,
            };
            
            var response = await httpClient.PostAsJsonAsync($"{ApiUri}/add", newEmployee);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
    
    public async Task<bool> UpdateEmployee(EmployeeForm employee)
    {
        try
        {
            var updateEmployee = new EmployeeDto
            {
                Name = employee.Name,
                Surname = employee.Surname,
                BirthDate = employee.BirthDate,
                Position = employee.Position,
                IpAddress = employee.IpAddress,
            };
            
            var response = await httpClient.PutAsJsonAsync($"{ApiUri}/{employee.Id}", updateEmployee);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
    
    public async Task<bool> DeleteEmployee(int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{ApiUri}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
    
    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Employee>>(ApiUri);
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
    
    public async Task<IEnumerable<string>> GetPositions()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<string>>($"{ApiUri}/positions");
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
    
    public async Task<HttpResponseMessage> AddEmployeesJson(EmployeesDto json)
    {
        try
        {
            return await httpClient.PostAsJsonAsync($"{ApiUri}/add/json", json);
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }

    public async Task<HttpResponseMessage> AddPositionsJson(PositionsDto json)
    {
        try
        {
            return await httpClient.PostAsJsonAsync($"{ApiUri}/add/json/positions", json);
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
}