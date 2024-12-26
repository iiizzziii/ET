using ET.Dto;

namespace ET.Web.Services;

public interface IEmployeeService
{
    // Task<EmployeeDto?> GetEmployee(int id);
    Task<IEnumerable<EmployeeDto>?> GetEmployees();
}