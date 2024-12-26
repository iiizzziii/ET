using ET.Dto;
using Microsoft.AspNetCore.Components;

namespace ET.Web.Pages;

public class DisplayEmployeesBase : ComponentBase
{
    [Parameter]
    public IEnumerable<EmployeeDto>? Employees { get; set; }
}