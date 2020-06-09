using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Widgets.Employees.Controllers;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
#if !NOP_PRE_4_3
using Microsoft.AspNetCore.Builder;
#endif

namespace Nop.Plugin.Widgets.Employees
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(
#if NOP_PRE_4_3
            IRouteBuilder builder
#else
            IEndpointRouteBuilder builder
#endif
            )
        {
            RegisterPublicRoutes(builder);
            RegisterDepartmentAdminRoutes(builder);
            RegisterEmployeeAdminRoutes(builder);
        }

        private void RegisterPublicRoutes(
#if NOP_PRE_4_3
            IRouteBuilder builder
#else
            IEndpointRouteBuilder builder
#endif
            )
        {
            string controller = EmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Employees";

            string action = "";
            string actionSanitized = "";

            void Build(string routeAction = null) =>
#if NOP_PRE_4_3
                builder.MapLocalizedRoute(
                        name:$"{namePrefix}{action}",
                        template:routeAction ?? $"{routePrefix}/{actionSanitized}",
                        defaults:new { controller = controller, action = action });
#else
                builder.MapControllerRoute(
                        name: $"{namePrefix}{action}",
                        pattern: routeAction ?? $"{routePrefix}/{actionSanitized}",
                        defaults: new { controller = controller, action = action });
#endif

            // Index
            action = nameof(EmployeesController.Index);
            actionSanitized = action + "/{groupByDepartment}";
            Build(routePrefix); // Skip action - to get only /Employees

            // Employee info
            action = nameof(EmployeesController.Info);
            actionSanitized = action + "/{id}";
            Build();
        }

        private void RegisterDepartmentAdminRoutes(
#if NOP_PRE_4_3
            IRouteBuilder builder
#else
            IEndpointRouteBuilder builder
#endif
            )
        {
            string controller = DepartmentsController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Admin/Departments";

            string action = "";
            string actionSanitized = "";

            void Build() =>
#if NOP_PRE_4_3
                builder.MapLocalizedRoute(
                                name:$"{namePrefix}{action}",
                                template:$"{routePrefix}/{actionSanitized}",
                                defaults:new { controller = controller, action = action, area = "Admin" });
#else
                builder.MapAreaControllerRoute(
                                name: $"{namePrefix}{action}",
                                areaName: "Admin",
                                pattern: $"{routePrefix}/{actionSanitized}",
                                defaults: new { controller = controller, action = action, area = "Admin" });
#endif

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

        private void RegisterEmployeeAdminRoutes(
#if NOP_PRE_4_3
            IRouteBuilder builder
#else
            IEndpointRouteBuilder builder
#endif
            )
        {
            string controller = EmployeesController.ControllerName;
            string namePrefix = $"Plugins.{controller}.";
            const string routePrefix = "Admin/Employees";

            string action = "";
            string actionSanitized = "";

            void Build() =>
#if NOP_PRE_4_3
                builder.MapLocalizedRoute(
                                name:$"{namePrefix}{action}",
                                template:$"{routePrefix}/{actionSanitized}",
                                defaults:new { controller = controller, action = action, area = "Admin" });
#else
                builder.MapAreaControllerRoute(
                                name: $"{namePrefix}{action}",
                                areaName: "Admin",
                                pattern: $"{routePrefix}/{actionSanitized}",
                                defaults: new { controller = controller, action = action, area = "Admin" });
#endif

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

            // List Employees
            action = nameof(EmployeesController.List);
            actionSanitized = action;
            Build();

            // List employee data
            action = nameof(EmployeesController.ListData);
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
