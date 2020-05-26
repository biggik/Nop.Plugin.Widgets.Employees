using Microsoft.AspNetCore.Builder;
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
            string action = "";
            string actionSanitized = "";
            string namePrefix = "";

            ////////////////////////////////////////////////
            // Public methods
            string controller = WidgetsEmployeesController.ControllerName;
            namePrefix = $"Plugins.{controller}.";

            // Index
            action = nameof(WidgetsEmployeesController.Index);
            actionSanitized = action;
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}", // Skip action - to get only /Employees
                new { controller = controller, action = action });

            // Employee info
            action = nameof(WidgetsEmployeesController.EmployeeInfo);
            actionSanitized = "Info/{id}";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action });

            ////////////////////////////////////////////////
            // Admin methods
            controller = WidgetsEmployeesController.ControllerName;
            namePrefix = $"Plugins.{controller}.";

            // Configure
            action = nameof(WidgetsEmployeesController.Configure);
            actionSanitized = "Configure";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action, area = "Admin" });

            // Edit department
            action = nameof(WidgetsEmployeesController.EditDepartment);
            actionSanitized = "Department/Edit/{id}";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action, area = "Admin" });

            // Edit employee
            action = nameof(WidgetsEmployeesController.EditEmployee);
            actionSanitized = "Employee/Edit/{id}";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action, area = "Admin" });

            // Create department
            action = nameof(WidgetsEmployeesController.CreateDepartment);
            actionSanitized = "Department/Create";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action, area = "Admin" });

            // Create employee
            action = nameof(WidgetsEmployeesController.CreateEmployee);
            actionSanitized = "Employee/Create";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action, area = "Admin" });

            // Create employee
            action = nameof(WidgetsEmployeesController.GetFullSizeEmployeePicture);
            actionSanitized = "Employee/Picture/{id}";
            routeBuilder.MapLocalizedRoute($"{namePrefix}{action}",
                $"{WidgetsEmployeesController.UrlRouteName}/{actionSanitized}",
                new { controller = controller, action = action, area = "Admin" });
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
