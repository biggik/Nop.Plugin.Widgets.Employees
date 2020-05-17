using System.Collections.Generic;
using Nop.Core;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial interface IEmployeesService
    {
        void DeleteEmployeesRecord(EmployeesRecord employee);

        IPagedList<EmployeesRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);

        EmployeesRecord GetById(int id);

        void InsertEmployeesRecord(EmployeesRecord employee);
        void InsertDepartmentRecord(DepartmentRecord department);

        void UpdateEmployeesRecord(EmployeesRecord employee);
        void UpdateDepartmentRecord(DepartmentRecord department);

        IList<DepartmentRecord> GetAllDepartments(bool showHidden = false);

        IList<EmployeesRecord> GetEmployeeByDepartmentId(int departmentId);

        DepartmentRecord GetDepartmentByDepartmentId(int departId);
    }
}
