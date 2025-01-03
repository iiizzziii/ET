using System.Net;
using System.Text.Json;
using ET.Models;
using Microsoft.AspNetCore.Components;

#pragma warning disable CA1869

namespace ET.Web.Pages;

public partial class JsonEmployees : ComponentBase
{
    private bool _popupVisible;
    private string _jsonInput = string.Empty;
    private string _message = string.Empty;
    private string _popupMessage = string.Empty;

    private async Task UploadJson()
    {
        if (string.IsNullOrWhiteSpace(_jsonInput)) {
            _message = "Please provide valid JSON input.";
            return; }

        try
        {
            var employees = JsonSerializer.Deserialize<EmployeesDto>(
                _jsonInput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var response = await EmployeeService.AddEmployeesJson(employees!);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _message = "Nothing to add, all positions already exist";
            }
            else if (response.IsSuccessStatusCode)
            {
                _popupMessage = await response.Content.ReadAsStringAsync();
                await ShowSuccessPopup();
                NavigationManager.NavigateTo("/employees");
            }
            else
            {
                _message =  "Failed to upload positions. Please check your input.";
            }
        }
        catch (JsonException) { _message = "Invalid JSON format."; }
        catch (Exception ex) { _message = $"An error occurred: {ex.Message}"; }
    }
    
    private async Task ShowSuccessPopup()
    {
        _popupVisible = true;
        StateHasChanged();
        await Task.Delay(3000);
        _popupVisible = false;
        StateHasChanged();
    }
}