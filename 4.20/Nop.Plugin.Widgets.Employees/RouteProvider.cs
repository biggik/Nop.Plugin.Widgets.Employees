using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.Employees
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapLocalizedRoute("Plugin.Widgets.Employees.List", "Employees/List",
                 new { controller = "WidgetsEmployees", action = "List" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Widgets.Employees.Info", "Employees/EmployeeInfo/{id}",
                 new { controller = "WidgetsEmployees", action = "EmployeeInfo" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Widgets.Employees.Delete", "Employees/EmployeeDelete/{id}",
                 new { controller = "WidgetsEmployees", action = "EmployeeDelete" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Widgets.Employees.EditEmployee", "Employees/EditEmployee/{id}",
                 new { controller = "WidgetsEmployees", action = "EditEmployee" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Widgets.Employees.CreateEmployee", "Employees/CreateEmployee",
                 new { controller = "WidgetsEmployees", action = "CreateEmployee" }
            );

            routeBuilder.MapLocalizedRoute("Plugin.Widgets.Employees.CreateDepartment", "Employees/CreateDepartment",
                 new { controller = "WidgetsEmployees", action = "CreateDepartment" }
            );
        }

        public int Priority
        {
            get
            {
                return -1;
            }
        }
    }
}
