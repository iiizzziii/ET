using System.Net.Http.Json;
using ET.Models;

namespace ET.Web.Services;

public class EmployeeService(HttpClient httpClient) : IEmployeeService
{
    // public async Task<EmployeeDto?> GetEmployee(int id)
    // {
    //     try
    //     {
    //         var response = await httpClient.GetAsync($"api/employees/{id}");
    //         if (response.IsSuccessStatusCode)
    //         {
    //             return await response.Content.ReadFromJsonAsync<EmployeeDto>();
    //         }
    //         throw new Exception(await response.Content.ReadAsStringAsync());
    //     }
    //     catch (Exception e) {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }

    public async Task<IEnumerable<Employee>?> GetEmployees()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<Employee>>("api/employees");
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteEmployee(int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/employees/{id}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
}