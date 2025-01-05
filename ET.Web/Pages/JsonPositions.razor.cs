using ET.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

#pragma warning disable CA1869

namespace ET.Web.Pages;

public partial class JsonPositions : ComponentBase
{
    private bool _popupVisible;
    private string _jsonInput = string.Empty;
    private string _message = string.Empty;
    private string _popupMessage = string.Empty;
    private bool isDragging;

    private PositionsDto? _positions;
    private string? _validationMessage;
    private string? _responseContent;

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null) return;

        if (file.ContentType != "application/json")
        {
            _validationMessage = "invalid file type";
            _positions = null;
            return;
        }
        
        // await using FileStream fs = new(path, FileMode.Create);
        // await browserFile.OpenReadStream().CopyToAsync(fs);
        
        using var fileStream = file.OpenReadStream(); 
        using var streamReader = new StreamReader(fileStream);
        var jsonContent = await streamReader.ReadToEndAsync();

        try
        {
            _positions = JsonSerializer.Deserialize<PositionsDto>(
                jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _validationMessage = null;
        }
        catch (JsonException)
        {
            _positions = null;
            _validationMessage = "json not valid";
        }
    }
    
    private async Task SendToBackend()
    {
        var response = await EmployeeService.AddPositionsJson(_positions);
        _responseContent = await response.Content.ReadAsStringAsync();
    }
    

    // private async Task OnDrop(
    //     // DragEventArgs e
    //     InputFileChangeEventArgs e)
    // {
    //     isDragging = false;
    //
    //     // if (e.DataTransfer.Files.Length > 0)
    //     // {
    //     //     var file = e.DataTransfer.Files[0];
    //         try
    //         {
    //             var file = e.File;
    //             
    //             _ = JsonDocument.Parse(file.ContentType);
    //             var stream = new MemoryStream();
    //             await e.File.OpenReadStream().CopyToAsync(stream);
    //             _jsonInput = stream.ToString();
    //             
    //             // using var streamReader = new StreamReader(file.ToString());
    //             // _jsonInput = await streamReader.ReadToEndAsync();
    //             
    //             var positions = JsonSerializer.Deserialize<PositionsDto>(
    //                 _jsonInput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    //             
    //             var response = await EmployeeService.AddPositionsJson(positions!);
    //         
    //             if (response.StatusCode == HttpStatusCode.NoContent)
    //             {
    //                 _message = "Nothing to add, all positions already exist";
    //             }
    //             else if (response.IsSuccessStatusCode)
    //             {
    //                 _popupMessage = await response.Content.ReadAsStringAsync();
    //                 await ShowSuccessPopup();
    //                 NavigationManager.NavigateTo("/employees");
    //             }
    //             else
    //             {
    //                 _message =  "Failed to upload positions. Please check your input.";
    //             }
    //         }
    //         catch (JsonException) { _message = "Please provide valid JSON file."; }
    //         catch (Exception ex) { _message = $"An error occurred: {ex.Message}"; }
    //     // }
    // }
    
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