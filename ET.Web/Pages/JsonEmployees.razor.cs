using ET.Models;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

#pragma warning disable CA1869

namespace ET.Web.Pages;

public partial class JsonEmployees : ComponentBase
{
    private EmployeesDto? _employees;
    private string? _validationMessage;
    private string? _responseContent;

    private async Task HandleFile(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file.ContentType != "application/json")
        {
            _validationMessage = "Chosen file is not JSON.";
            _employees = null;
            return;
        }

        await using var fileStream = file.OpenReadStream(); 
        using var streamReader = new StreamReader(fileStream);
        var jsonContent = await streamReader.ReadToEndAsync();

        try
        {
            _employees = JsonSerializer.Deserialize<EmployeesDto>(
                jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _validationMessage = null;
        }
        catch (JsonException)
        {
            _employees = null;
            _validationMessage = "JSON not valid.";
        }
    }
    
    private async Task UploadJson()
    {
        try
        {
            var response = await EmployeeService.AddEmployeesJson(_employees!);

            _responseContent = response.StatusCode == HttpStatusCode.NoContent
                ? 
                "Nothing to add, all employees already exist"
                : 
                response.IsSuccessStatusCode
                    ? 
                    await response.Content.ReadAsStringAsync()
                    : 
                    "Failed to upload employees. Please check your input.";
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
}