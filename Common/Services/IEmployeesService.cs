using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial interface IEmployeesService
    {
        Task<IPagedList<Employee>> GetOrderedEmployeesAsync(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<IPagedList<Department>> GetOrderedDepartmentsAsync(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<Employee> GetByIdAsync(int id);
        Task<Employee> GetByEmailPrefixAsync(string emailPrefix);

        Task InsertEmployeeAsync(Employee employee);
        Task InsertDepartmentAsync(Department department);

        Task UpdateEmployeeAsync(Employee employee);
        Task UpdateDepartmentAsync(Department department);

        Task DeleteEmployeeAsync(Employee employee);
        Task DeleteDepartmentAsync(Department department);

        Task<IList<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId, bool showUnpublished = false);

        Task<Department> GetDepartmentByIdAsync(int departmentId);
    }
}
