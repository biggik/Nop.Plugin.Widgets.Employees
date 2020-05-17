using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Employees.Models
{
    public class DepartmentEmployeeModel
    {
        public DepartmentEmployeeModel()
        {
            EmployeesList = new List<EmployeesListModel>();
        }

        public bool IsAdmin { get; set; }

        public List<EmployeesListModel> EmployeesList { get; set; }
    }
}