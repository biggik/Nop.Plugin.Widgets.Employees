using System.Collections.Generic;
using Nop.Services.Security;

namespace Nop.Plugin.Widgets.Employees.Services;

public partial class EmployeePermissionsConfigProvider : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs
        => [EmployeePermissionConfigs.ManageEmployees,
            EmployeePermissionConfigs.ManageDepartments];
}