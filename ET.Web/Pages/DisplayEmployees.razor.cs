using ET.Models;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ET.Web.Pages;

public partial class DisplayEmployees : ComponentBase
{
    [Parameter]
    public IEnumerable<Employee>? employees { get; set; }
    private IEnumerable<string> positions;
    private EmployeeForm editEmployee;
    private Employee? _viewEmployee;
    private bool visibleModal;
    private Dictionary<string, List<string>> validationErrors = new();
    
    protected override async Task OnInitializedAsync()
    {
        employees = await EmployeeService.GetEmployees();
        positions = await EmployeeService.GetPositions();
    }

    private void ShowModal(Employee employee)
    {
        editEmployee = new EmployeeForm
        {
            Id = employee.EmployeeId,
            Name = employee.Name,
            Surname = employee.Surname,
            BirthDate = employee.BirthDate,
            Position = employee.Position.PositionName,
            IpAddress = employee.IpAddress
        };
        validationErrors.Clear();
        visibleModal = true;
    }

    private void CloseModal() => visibleModal = false;
    
    private async Task UpdateEmployee()
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(editEmployee, serviceProvider: null, items: null);
    
        if (!Validator.TryValidateObject(editEmployee, validationContext, validationResults, true))
        {
            validationErrors.Clear();
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
            await EmployeeService.UpdateEmployee(editEmployee);
            employees = await EmployeeService.GetEmployees();
            visibleModal = false;
            StateHasChanged();
        }
        catch (Exception e) { Console.WriteLine(e); throw; }
    }
    
    private async Task DeleteEmployee(int id)
    {
        if (await EmployeeService.DeleteEmployee(id))
        {
            employees = await EmployeeService.GetEmployees();
        }
        else
        {
            Console.WriteLine("Failed to delete employee.");
        }
    }

    private void ViewEmployeeDetails(Employee employee)
    {
        _viewEmployee = employee;
        var parameters = new ModalParameters { { nameof(ViewEmployee.Employee), employee } };
        ModalService.Show<ViewEmployee>("Employee Details", parameters);
    }
}