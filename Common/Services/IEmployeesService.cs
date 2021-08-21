using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial interface IEmployeesService
    {
#if NOP_ASYNC
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
#else
        IPagedList<Employee> GetOrderedEmployees(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Department> GetOrderedDepartments(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue);

        Employee GetById(int id);
        Employee GetByEmailPrefix(string emailPrefix);

        void InsertEmployee(Employee employee);
        void InsertDepartment(Department department);

        void UpdateEmployee(Employee employee);
        void UpdateDepartment(Department department);

        void DeleteEmployee(Employee employee);
        void DeleteDepartment(Department department);

        IList<Employee> GetEmployeesByDepartmentId(int departmentId, bool showUnpublished = false);

        Department GetDepartmentById(int departmentId);

#endif
    }
}
