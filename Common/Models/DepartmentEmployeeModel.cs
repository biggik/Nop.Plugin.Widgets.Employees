using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Employees.Models
{
    public class DepartmentEmployeeModel
    {
        public DepartmentEmployeeModel()
        {
        }

        public bool GroupByDepartment { get; set; }

        public List<EmployeesListModel> EmployeesList { get; set; } = new List<EmployeesListModel>();
    }
}