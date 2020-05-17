using System.Collections.Generic;
using Nop.Web.Framework.Models;
namespace Nop.Plugin.Widgets.Employees.Models
{
    public class EmployeesListModel : BaseNopEntityModel
    {
        public EmployeesListModel()
        {
            Employees = new List<EmployeeModel>();
        }

        public string DepartmentName { get; set; }
        public List<EmployeeModel> Employees { get; set; }
    }
}