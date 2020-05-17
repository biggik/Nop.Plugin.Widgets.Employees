using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Security;
using Nop.Core;
using Nop.Services.Media;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    //[Area(AreaNames.Admin)]
    //[AuthorizeAdmin]
    public class WidgetsEmployeesController : BasePluginController
    {
        private readonly EmployeesSettings _employeeSettings;
        private readonly IEmployeesService _employeeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;

        public WidgetsEmployeesController(
            EmployeesSettings employeeSettings,
            IEmployeesService employeeService, 
            ISettingService settingService,
            ILocalizationService localizationService, 
            IPermissionService permissionService, 
            IWorkContext workContext, 
            IPictureService pictureService)
        {
            _employeeSettings = employeeSettings;
            _employeeService = employeeService;
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _workContext = workContext;
            _pictureService = pictureService;
        }
        
        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        public IActionResult List(bool groupByDepartment = false)
        {
            var model = new DepartmentEmployeeModel
            {
                IsAdmin = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                GroupByDepartment = groupByDepartment
            };

            foreach (var d in _employeeService.GetAllDepartments())
            {
                var employeesModel = new EmployeesListModel { DepartmentName = d.Name };
                foreach (var employee in _employeeService.GetEmployeesByDepartmentId(d.Id))
                {
                    var e = employee.ToModel();
                    e.PhotoUrl = GetPictureUrl(e.PictureId);
                    e.DepartmentName = d.Name;
                    employeesModel.Employees.Add(e);
                }
                model.EmployeesList.Add(employeesModel);
            }

            return View("~/Plugins/Widgets.Employees/Views/Employees/List.cshtml", model);
        }

        private string GetPictureUrl(int pictureId, int targetSize = 200)
        {
            return (pictureId > 0)
                ? _pictureService.GetPictureUrl(pictureId, targetSize)
                : _pictureService.GetDefaultPictureUrl(targetSize, Core.Domain.Media.PictureType.Avatar);
        }

        [HttpPost]
        public IActionResult EmployeeList(EmployeeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            var employees = _employeeService.GetAll(searchModel.Page - 1, searchModel.PageSize);
            var model = new EmployeeListModel().PrepareToGrid(searchModel, employees, () =>
            {
                return employees.Select(employee =>
                {
                    var e = employee.ToModel();
                    e.PhotoUrl = GetPictureUrl(e.PictureId);
                    return e;
                });
            });

            return Json(model);
        }

        public IActionResult EmployeeInfo(string id)
        {
            var model = new EmployeeInfoModel
            {
                IsAdmin = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel)
            };

            Domain.Employee e;
            if (int.TryParse(id, out int employeeId))
            {
                e = _employeeService.GetById(employeeId);
            }
            else
            {
                e = _employeeService.GetByEmailPrefix(id);
            }

            model.Employee = e.ToModel();
            model.Employee.PhotoUrl = (e.PictureId > 0) ? _pictureService.GetPictureUrl(e.PictureId, 200) : null;
            var department = _employeeService.GetDepartmentById(e.DepartmentId);
            if (department != null)
            {
                model.Employee.DepartmentName = department.Name;
            }
            return View("~/Plugins/Widgets.Employees/Views/Employees/EmployeeInfo.cshtml", model);
        }
    }
}
