using System.Net.Http.Json;
using ET.Dto;

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

    public async Task<IEnumerable<EmployeeDto>?> GetEmployees()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<IEnumerable<EmployeeDto>>("api/employees");
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
}