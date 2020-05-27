using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Widgets.Employees.Controllers;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.Employees
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            RegisterPublicRoutes(routeBuilder);
            RegisterDepartmentAdminRoutes(routeBuilder);
            RegisterEmployeeAdminRoutes(routeBuilder);
        }

        private void RegisterPublicRoutes(IRouteBuilder routeBuilder)
        {
            string controller = WidgetsEmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            string routePrefix = "Employees";

            string action = "";
            string actionSanitized = "";

            void Build(string routeAction = null) 
                => routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                        routeAction ?? $"{routePrefix}/{actionSanitized}",
                        new { controller = controller, action = action });

            // Index
            action = nameof(WidgetsEmployeesController.Index);
            actionSanitized = action + "/{groupByDepartment}";
            Build(routePrefix); // Skip action - to get only /Employees

            // ListAll
            action = nameof(WidgetsEmployeesController.ListAll);
            actionSanitized = action + "/{groupByDepartment}";
            Build();

            // Employee info
            action = nameof(WidgetsEmployeesController.EmployeeInfo);
            actionSanitized = "{id}";
            Build();
        }

        private void RegisterDepartmentAdminRoutes(IRouteBuilder routeBuilder)
        {
            string controller = WidgetsEmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            string routePrefix = "Admin/EmployeeDepartments";

            string action = "";
            string actionSanitized = "";

            void Build() => routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                                $"{routePrefix}/{actionSanitized}",
                                new { controller = controller, action = action, area = "Admin" });

            // Create department
            action = nameof(WidgetsEmployeesController.CreateDepartment);
            actionSanitized = "Create";
            Build();

            // Edit department
            action = nameof(WidgetsEmployeesController.EditDepartment);
            actionSanitized = "Edit/{id}";
            Build();

            // List departments
            action = nameof(WidgetsEmployeesController.DepartmentList);
            actionSanitized = "List";
            Build();

            // List department data
            action = nameof(WidgetsEmployeesController.DepartmentListData);
            actionSanitized = "ListData";
            Build();
        }

        private void RegisterEmployeeAdminRoutes(IRouteBuilder routeBuilder)
        {
            string controller = WidgetsEmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            string routePrefix = "Admin/Employees";

            string action = "";
            string actionSanitized = "";

            void Build() => routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                                $"{routePrefix}/{actionSanitized}",
                                new { controller = controller, action = action, area = "Admin" });

            // Configure
            action = nameof(WidgetsEmployeesController.Configure);
            actionSanitized = "Configure";
            Build();

            // Edit employee
            action = nameof(WidgetsEmployeesController.EditEmployee);
            actionSanitized = "Edit/{id}";
            Build();

            // Create employee
            action = nameof(WidgetsEmployeesController.CreateEmployee);
            actionSanitized = "Create";
            Build();

            // Full size employee picture
            action = nameof(WidgetsEmployeesController.GetFullSizeEmployeePicture);
            actionSanitized = "{id}/Picture";
            Build();
        }

        public int Priority => -1;
    }
}
