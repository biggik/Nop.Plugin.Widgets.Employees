using System.Linq;
using System.Reflection;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class EmployeesController 
    {
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return BadRequest();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<EmployeeWidgetSettings>(storeScope);

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.value, y => y.id);

            var currentWidgetZones = (from i in (settings.WidgetZones ?? "").Split(';')
                                      where lookup.ContainsKey(i)
                                      select lookup[i]).ToList();

            var model = new ConfigurationModel
            {
                WidgetZones = currentWidgetZones,
                AvailableWidgetZones = (from wzd in widgetZonesData
                                        select new SelectListItem
                                        {
                                            Text = wzd.name,
                                            Value = wzd.id.ToString(),
                                            Selected = currentWidgetZones.Contains(wzd.id)
                                        }
                                       ).ToList()
            };

            return View($"{Route}{nameof(Configure)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return BadRequest();

            if (!ModelState.IsValid)
                return Configure();

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<EmployeeWidgetSettings>(storeScope);

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.id, y => y.value);

            settings.WidgetZones = model.WidgetZones != null && model.WidgetZones.Any()
                ? string.Join(";",
                        from i in model.WidgetZones
                        where lookup.ContainsKey(i)
                        select lookup[i]
                        )
                : "";

            _settingService.SaveSetting(settings, s => s.WidgetZones, clearCache: false);
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult List()
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            return View($"{Route}{nameof(List)}.cshtml", new EmployeeSearchModel());
        }

        private List<(string name, string value, int id)> GetWidgetZoneData()
        {
            int id = 1000;
            return typeof(Nop.Web.Framework.Infrastructure.PublicWidgetZones)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .OrderBy(x => x.Name)
                .Select(x => (name: x.Name, value: x.GetValue(null, null).ToString(), id++))
                .ToList();
        }

        private IList<SelectListItem> GetAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return (from d in _employeeService.GetAllDepartments(showUnpublished: true)
                    .Where(dep => dep.Id == selectedDepartmentId || dep.Published)
                    select new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString(),
                        Selected = d.Id == selectedDepartmentId
                    })
                    .ToList();
        }

        private EmployeeModel GetEmployeeWithAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return new EmployeeModel
            {
                AvailableDepartments = GetAllAvailableDepartments(selectedDepartmentId)
            };
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var employee = _employeeService.GetById(id);
            if (employee != null)
                _employeeService.DeleteEmployee(employee);

            return RedirectToAction(nameof(Index), new { area = "" });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Create()
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var model = GetEmployeeWithAllAvailableDepartments();
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(EmployeeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

                _employeeService.InsertEmployee(employee);
                return continueEditing
                    ? RedirectToAction(nameof(Info), new { id = employee.Id, area = "" })
                    : RedirectToAction(nameof(Index), ControllerName, new { area = "" });
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(Index), new { area = "" });
            }

            var model = employee.ToModel();
            model.AvailableDepartments = GetAllAvailableDepartments(employee.DepartmentId);
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Edit(EmployeeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = _employeeService.GetById(model.Id);
                if (employee != null)
                {
                    employee = model.ToEntity(employee);
                }

                _employeeService.UpdateEmployee(employee);
                return continueEditing
                    ? RedirectToAction(nameof(Info), new { id = employee.Id, area = "" })
                    : RedirectToAction(nameof(Index), ControllerName, new { area = "" });
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult GetFullSizeEmployeePicture(int id)
        {
            if (id > 0)
            {
                var pic = _pictureService.GetPictureById(id);
#if NOP_PRE_4_3
                return File(pic.PictureBinary.BinaryData, pic.MimeType);
#else
                var binary = _pictureService.GetPictureBinaryByPictureId(id);
                return File(binary.BinaryData, pic.MimeType);
#endif
            }
            return NotFound();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult ListData(DepartmentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var departmentDict = _employeeService.GetAllDepartments(showUnpublished: true).ToDictionary(x => x.Id, x => x);

            var employees = _employeeService.GetAll(showUnpublished: true, searchModel.Page - 1, searchModel.PageSize);
            var model = new EmployeeListModel().PrepareToGrid(searchModel, employees, () =>
            {
                return employees.Select(employee =>
                {
                    var e = employee.ToModel();
                    e.DepartmentName = departmentDict[e.DepartmentId].Name;
                    e.DepartmentPublished = departmentDict[e.DepartmentId].Published;
                    return e;
                });
            });

            return Json(model);
        }

    }
}
