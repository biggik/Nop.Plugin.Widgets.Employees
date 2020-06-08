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
            string controller = EmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Employees";

            string action = "";
            string actionSanitized = "";

            void Build(string routeAction = null) 
                => routeBuilder.MapLocalizedRoute(
                        name:$"{namePrefix}{action}",
                        template:routeAction ?? $"{routePrefix}/{actionSanitized}",
                        defaults:new { controller = controller, action = action });

            // Index
            action = nameof(EmployeesController.Index);
            actionSanitized = action + "/{groupByDepartment}";
            Build(routePrefix); // Skip action - to get only /Employees

            // ListAll
            action = nameof(EmployeesController.ListAll);
            actionSanitized = action + "/{groupByDepartment}";
            Build();

            // Employee info
            action = nameof(EmployeesController.Info);
            actionSanitized = action + "/{id}";
            Build();
        }

        private void RegisterDepartmentAdminRoutes(IRouteBuilder routeBuilder)
        {
            string controller = DepartmentsController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Admin/Departments";

            string action = "";
            string actionSanitized = "";

            void Build() => routeBuilder.MapLocalizedRoute(
                                name:$"{namePrefix}{action}",
                                template:$"{routePrefix}/{actionSanitized}",
                                defaults:new { controller = controller, action = action, area = "Admin" });

            // Create department
            action = nameof(DepartmentsController.Create);
            actionSanitized = action;
            Build();

            // Edit department
            action = nameof(DepartmentsController.Edit);
            actionSanitized = action + "/{id}";
            Build();

            // List departments
            action = nameof(DepartmentsController.List);
            actionSanitized = action;
            Build();

            // List department data
            action = nameof(DepartmentsController.ListData);
            actionSanitized = action;
            Build();
        }

        private void RegisterEmployeeAdminRoutes(IRouteBuilder routeBuilder)
        {
            string controller = EmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Admin/Employees";

            string action = "";
            string actionSanitized = "";

            void Build() => routeBuilder.MapLocalizedRoute(
                                name:$"{namePrefix}{action}",
                                template:$"{routePrefix}/{actionSanitized}",
                                defaults:new { controller = controller, action = action, area = "Admin" });

            // Configure
            action = nameof(EmployeesController.Configure);
            actionSanitized = action;
            Build();

            // Edit employee
            action = nameof(EmployeesController.Edit);
            actionSanitized = action + "/{id}";
            Build();

            // Create employee
            action = nameof(EmployeesController.Create);
            actionSanitized = action;
            Build();

            // Full size employee picture
            action = nameof(EmployeesController.GetFullSizeEmployeePicture);
            actionSanitized = "{id}/Picture";
            Build();
        }

        public int Priority => 1000;
    }
}
