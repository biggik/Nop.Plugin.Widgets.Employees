using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial interface IEmployeesService
    {
        IPagedList<Employee> GetAll(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Department> GetAllDepartments(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue);

        Employee GetById(int id);
        Employee GetByEmailPrefix(string emailPrefix);

        void InsertEmployee(Employee employee);
        void InsertDepartment(Department department);

        void UpdateEmployee(Employee employee);
        void UpdateDepartment(Department department);

        void DeleteEmployee(Employee employee);

        IList<Employee> GetEmployeesByDepartmentId(int departmentId, bool showUnpublished = false);

        Department GetDepartmentById(int departId);
    }
}
