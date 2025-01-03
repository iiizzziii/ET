using System.ComponentModel.DataAnnotations;
using ET.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ET.Web.Pages;

public partial class AddEmployee : ComponentBase
{
    private readonly EmployeeForm _newEmployee = new();
    private Dictionary<string, List<string>> validationErrors = new();
    private IEnumerable<string> _positions;
    private string _selectedPosition;
    private EditContext _editContext;

    protected override async Task OnInitializedAsync()
    {
        _positions = await EmployeeService.GetPositions();
        _editContext = new EditContext(_newEmployee);
    }

    private async Task HandleValidSubmit()
    {
        if (!string.IsNullOrEmpty(_selectedPosition)) {
            _newEmployee.Position = _positions.FirstOrDefault(p => p == _selectedPosition); }
        
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(_newEmployee);

        if (!Validator.TryValidateObject(_newEmployee, validationContext, validationResults, true))
        {
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    if (!validationErrors.ContainsKey(memberName))
                    {
                        validationErrors[memberName] = new List<string>();
                    }
                    validationErrors[memberName].Add(validationResult.ErrorMessage);
                }
            }
            return;
        }

        try
        {
            await EmployeeService.AddEmployee(_newEmployee);
            NavigationManager.NavigateTo("/employees");
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
}