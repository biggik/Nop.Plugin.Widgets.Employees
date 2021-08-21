﻿using Nop.Plugin.Widgets.Employees.Models;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Core;
using Nop.Services.Media;
using Nop.Services.Messages;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class EmployeesController : BasePluginController
    {
        public static string ControllerName = nameof(EmployeesController).Replace("Controller", "");
        const string Route = "~/Plugins/Widgets.Employees/Views/Employees/";

        private readonly IEmployeesService _employeeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;

        public EmployeesController(
            IEmployeesService employeeService,
            IStoreContext storeContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IPictureService pictureService)
        {
            _employeeService = employeeService;
            _storeContext = storeContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _workContext = workContext;
            _pictureService = pictureService;
        }

#if NOP_ASYNC
        public async Task<IActionResult> Index(bool groupByDepartment = true)
#else
        public IActionResult Index(bool groupByDepartment = true)
#endif
        {
            var model = new DepartmentEmployeeModel
            {
                GroupByDepartment = groupByDepartment
            };

            var employesModel = new EmployeesListModel { Department = new DepartmentModel { Name = "" } };

            var emailCount = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var d in
#if NOP_ASYNC
                await _employeeService.GetOrderedDepartmentsAsync
#else
                _employeeService.GetOrderedDepartments
#endif
                    (showUnpublished: false))
            {
                if (groupByDepartment)
                {
                    employesModel = new EmployeesListModel { Department = d.ToModel() };
                }
                foreach (var employee in
#if NOP_ASYNC
                await _employeeService.GetEmployeesByDepartmentIdAsync
#else
                _employeeService.GetEmployeesByDepartmentId
#endif
                    (d.Id, showUnpublished: false))
                {
                    var e = employee.ToModel();

                    if (emailCount.ContainsKey(e.Email))
                    {
                        emailCount[e.Email]++;
                    }
                    else
                    {
                        emailCount[e.Email] = 1;
                    }

#if NOP_ASYNC
                    e.PhotoUrl = await GetPictureUrl(e.PictureId);
#else
                    e.PhotoUrl = GetPictureUrl(e.PictureId);
#endif
                    e.DepartmentPublished = d.Published;
                    e.DepartmentName = d.Name;

                    employesModel.Employees.Add(e);
                }
                if (groupByDepartment)
                {
                    // The Model is a list of departments each with a list of employees
                    model.EmployeesList.Add(employesModel);
                }
            }
            if (!groupByDepartment)
            {
                // The Model is a single-entry list (empty department) with a list of all employees
                model.EmployeesList.Add(employesModel);
            }

            foreach (var employeeList in model.EmployeesList)
            {
                foreach (var employee in employeeList.Employees)
                {
                    employee.HasUniqueEmail = emailCount[employee.Email] == 1;
                }
            }

#if NOP_ASYNC
            if (await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
            {
                DisplayEditLink(Url.Action(nameof(List), ControllerName, new { area = "Admin" }));
            }

            return View($"{Route}{nameof(Index)}.cshtml", model);
        }

#if NOP_ASYNC
        private async Task<string> GetPictureUrl(int pictureId, int targetSize = 200)
        {
            return (pictureId > 0)
                ? await _pictureService.GetPictureUrlAsync(pictureId, targetSize)
                : await _pictureService.GetDefaultPictureUrlAsync(targetSize, Core.Domain.Media.PictureType.Avatar);
        }
#else
        private string GetPictureUrl(int pictureId, int targetSize = 200)
        {
            return (pictureId > 0)
                ? _pictureService.GetPictureUrl(pictureId, targetSize)
                : _pictureService.GetDefaultPictureUrl(targetSize, Core.Domain.Media.PictureType.Avatar);
        }
#endif

#if NOP_ASYNC
        public async Task<IActionResult> Info(string id)
#else
        public IActionResult Info(string id)
#endif
        {
            var model = new EmployeeInfoModel
            {
#if NOP_ASYNC
                IsAdmin = await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees)
#else
                IsAdmin = _permissionService.Authorize(EmployeePermissionProvider.ManageEmployees)
#endif
            };

#if NOP_ASYNC
            var e = await GetEmployeeByIdOrEmailPrefix(id);
#else
            var e = GetEmployeeByIdOrEmailPrefix(id);
#endif
            if (e == null)
            {
                return RedirectToAction(nameof(Index), new { area = "" });
            }

            if (model.IsAdmin)
            {
                DisplayEditLink(Url.Action(nameof(Edit), ControllerName, new { id = e.Id.ToString(), area = "Admin" }));
            }

            model.Employee = e.ToModel();
#if NOP_ASYNC
            model.Employee.PhotoUrl = (e.PictureId > 0) ? await _pictureService.GetPictureUrlAsync(e.PictureId, 200) : null;
            var department = await _employeeService.GetDepartmentByIdAsync(e.DepartmentId);
#else
            model.Employee.PhotoUrl = (e.PictureId > 0) ? _pictureService.GetPictureUrl(e.PictureId, 200) : null;
            var department = _employeeService.GetDepartmentById(e.DepartmentId);
#endif
            if (department != null)
            {
                model.Employee.DepartmentPublished = department.Published;
                model.Employee.DepartmentName = department.Name;
            }
            return View($"{Route}{nameof(Info)}.cshtml", model);
        }

#if NOP_ASYNC
        private async Task<Domain.Employee> GetEmployeeByIdOrEmailPrefix(string id)
        {
            if (int.TryParse(id, out int employeeId))
            {
                return await _employeeService.GetByIdAsync(employeeId);
            }
            return await _employeeService.GetByEmailPrefixAsync(id);
        }
#else
        private Domain.Employee GetEmployeeByIdOrEmailPrefix(string id)
        {
            if (int.TryParse(id, out int employeeId))
            {
                return _employeeService.GetById(employeeId);
            }
            return _employeeService.GetByEmailPrefix(id);
        }
#endif
    }
}
