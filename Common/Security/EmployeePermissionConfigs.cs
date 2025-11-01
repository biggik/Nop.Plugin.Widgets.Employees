using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Widgets.Employees.Services;

internal static class EmployeePermissionConfigs
{
    public const string MANAGE_EMPLOYEES = "ManageEmployees";
    public const string MANAGE_DEPARTMENTS = "ManageDepartments";

    internal static readonly PermissionConfig ManageEmployees = new(
        "Admin area. Manage Employees",
        MANAGE_EMPLOYEES,
        "Employees",
        NopCustomerDefaults.AdministratorsRoleName);

    internal static readonly PermissionConfig ManageDepartments = new(
        "Admin area. Manage Departments",
        MANAGE_DEPARTMENTS,
        "Employees",
        NopCustomerDefaults.AdministratorsRoleName);
}