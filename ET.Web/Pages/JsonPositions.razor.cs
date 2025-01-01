using System.Net;
using System.Text.Json;
using ET.Models;
using Microsoft.AspNetCore.Components;

#pragma warning disable CA1869

namespace ET.Web.Pages;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class JsonPositions : ComponentBase
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
            var positions = JsonSerializer.Deserialize<PositionsDto>(
                _jsonInput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
            var response = await EmployeeService.AddPositionsJson(positions!);
            
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
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(errorContent);
                _message = errorResponse?.Errors != null ? string.Join(
                        "; ", errorResponse.Errors.SelectMany(e => e.Value)) 
                    : "Failed to upload employees. Please check your input.";
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
    
    public class ValidationErrorResponse
    {
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}