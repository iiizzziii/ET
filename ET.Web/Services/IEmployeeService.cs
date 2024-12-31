using ET.Models;

namespace ET.Web.Services;

public interface IEmployeeService
{
    Task<bool> AddEmployee(EmployeeForm employee);
    Task<bool> UpdateEmployee(EmployeeForm employee);
    Task<bool> DeleteEmployee(int id);
    Task<IEnumerable<Employee>> GetEmployees();
    Task<IEnumerable<string>> GetPositions();
    Task<HttpResponseMessage> AddEmployeesJson(EmployeesDto json);
    Task<HttpResponseMessage> AddPositionsJson(PositionsDto json);
}