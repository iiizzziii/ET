using ET.Models;

namespace ET.Web.Services;

public interface IEmployeeService
{
    // Task<EmployeeDto?> GetEmployee(int id);
    Task<IEnumerable<Employee>?> GetEmployees();
    Task<bool> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(int id);
}