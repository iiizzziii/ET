using ET.Models;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace ET.Web.Pages;

public partial class ViewEmployee : ComponentBase
{
    [CascadingParameter]
    private IModalService _modalService { get; set; } = default!;
    
    [Parameter]
    public Employee Employee { get; set; }
}