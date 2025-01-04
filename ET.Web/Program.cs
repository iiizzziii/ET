using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ET.Web;
using ET.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient 
    { BaseAddress = new Uri("http://localhost:5093") });

builder.Services.AddBlazoredModal();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

await builder.Build().RunAsync();