using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial class EmployeePermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageEmployees = new PermissionRecord { Name = "Admin area. Manage Employees", SystemName = "ManageEmployees", Category = "Employees" };
        public static readonly PermissionRecord ManageDepartments = new PermissionRecord { Name = "Admin area. Manage Departments", SystemName = "ManageDepartments", Category = "Employees" };

        public virtual IEnumerable<PermissionRecord> GetPermissions() =>
            new[]
            {
                ManageEmployees,
                ManageDepartments
            };

        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions() =>
            new()
            { ("Administrators", new[] { ManageEmployees, ManageDepartments }) };
    }
}
