using ET.Models;

namespace ET.Web.Services;

public interface IEmployeeService
{
    // Task<EmployeeDto?> GetEmployee(int id);
    Task<IEnumerable<Employee>?> GetEmployees();
    Task<bool> UpdateEmployee(EmployeeForm employee);
    Task<bool> DeleteEmployee(int id);
    Task<IEnumerable<string>> GetPositions();
}