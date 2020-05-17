using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.Employees
{
    public class EmployeesSettings : ISettings
    {
        public bool LimitMethodsToCreated { get; set; }
    }
}