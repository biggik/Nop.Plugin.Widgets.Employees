using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Widgets.Employees.Controllers;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.AspNetCore.Builder;

namespace Nop.Plugin.Widgets.Employees
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder builder)
        {
            RegisterPublicRoutes(builder);
            RegisterDepartmentAdminRoutes(builder);
            RegisterEmployeeAdminRoutes(builder);
        }

        private void RegisterPublicRoutes(IEndpointRouteBuilder builder)
        {
            string controller = EmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            string routePrefix = controller;

            string action = "";
            string actionSanitized = "";

            void Build(string routeAction = null) =>
                builder.MapControllerRoute(
                        name: $"{namePrefix}{action}",
                        pattern: routeAction ?? $"{routePrefix}/{actionSanitized}",
                        defaults: new { controller = controller, action = action });

            // Index
            action = nameof(EmployeesController.IndexAsync);
            actionSanitized = action + "/{groupByDepartment}";
            Build(routePrefix); // Skip action - to get only /Employees

            // Employee info
            action = nameof(EmployeesController.InfoAsync);
            actionSanitized = action + "/{id}";
            Build();
        }

        private void RegisterDepartmentAdminRoutes(IEndpointRouteBuilder builder)
        {
            string controller = DepartmentsController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Admin/Departments";

            string action = "";
            string actionSanitized = "";

            void Build() =>
                builder.MapAreaControllerRoute(
                                name: $"{namePrefix}{action}",
                                areaName: "Admin",
                                pattern: $"{routePrefix}/{actionSanitized}",
                                defaults: new { controller = controller, action = action, area = "Admin" });

            // Create department
            action = nameof(DepartmentsController.CreateAsync);
            actionSanitized = action;
            Build();

            // Edit department
            action = nameof(DepartmentsController.EditAsync);
            actionSanitized = action + "/{id}";
            Build();

            // List departments
            action = nameof(DepartmentsController.ListAsync);
            actionSanitized = action;
            Build();

            // List department data
            action = nameof(DepartmentsController.ListDataAsync);
            actionSanitized = action;
            Build();
        }

        private void RegisterEmployeeAdminRoutes(IEndpointRouteBuilder builder)
        {
            string controller = EmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Admin/Employees";

            string action = "";
            string actionSanitized = "";

            void Build() =>
                builder.MapAreaControllerRoute(
                                name: $"{namePrefix}{action}",
                                areaName: "Admin",
                                pattern: $"{routePrefix}/{actionSanitized}",
                                defaults: new { controller = controller, action = action, area = "Admin" });

            // Configure
            action = nameof(EmployeesController.ConfigureAsync);
            actionSanitized = action;
            Build();

            // Edit employee
            action = nameof(EmployeesController.EditAsync);
            actionSanitized = action + "/{id}";
            Build();

            // Create employee
            action = nameof(EmployeesController.CreateAsync);
            actionSanitized = action;
            Build();

            // List Employees
            action = nameof(EmployeesController.ListAsync);
            actionSanitized = action;
            Build();

            // List employee data
            action = nameof(EmployeesController.ListDataAsync);
            actionSanitized = action;
            Build();

            // Full size employee picture
            action = nameof(EmployeesController.GetFullSizeEmployeePictureAsync);
            actionSanitized = "{id}/Picture";
            Build();
        }

        public int Priority => 1000;
    }
}
