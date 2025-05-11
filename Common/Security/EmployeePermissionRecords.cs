using Nop.Core.Domain.Security;

namespace Nop.Plugin.Widgets.Employees.Services;

internal static class EmployeePermissionRecords
{
    internal static readonly PermissionRecord ManageEmployees = new()
    {
        Name = "Admin area. Manage Employees",
        SystemName = "ManageEmployees",
        Category = "Employees"
    };

    internal static readonly PermissionRecord ManageDepartments = new()
    {
        Name = "Admin area. Manage Departments",
        SystemName = "ManageDepartments",
        Category = "Employees"
    };
}