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
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class EmployeesController 
    {
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
            {
                return AccessDeniedView();
            }
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);

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
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
            {
                return AccessDeniedView();
            }

            if (!ModelState.IsValid)
                return await Configure();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.id, y => y.value);

            settings.WidgetZones = model.WidgetZones != null && model.WidgetZones.Any()
                ? string.Join(";",
                        from i in model.WidgetZones
                        where lookup.ContainsKey(i)
                        select lookup[i]
                        )
                : "";

            await _settingService.SaveSettingAsync(settings, s => s.WidgetZones, clearCache: false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var model = new EmployeeSearchModel();
            model.SetGridPageSize();
            return View($"{Route}{nameof(List)}.cshtml", model);
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

        private async Task<IList<SelectListItem>> GetAllAvailableDepartmentsAsync(int selectedDepartmentId = -1)
        {
            return (from d in (await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true))
                    .Where(dep => dep.Id == selectedDepartmentId || dep.Published)
                    select new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString(),
                        Selected = d.Id == selectedDepartmentId
                    })
                    .ToList();
        }

        private async Task<EmployeeModel> GetEmployeeWithAllAvailableDepartmentsAsync(int selectedDepartmentId = -1)
        {
            return new EmployeeModel
            {
                AvailableDepartments = await GetAllAvailableDepartmentsAsync(selectedDepartmentId)
            };
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee != null)
                await _employeeService.DeleteEmployeeAsync(employee);

            return RedirectToAction(nameof(Index), new { area = "" });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var model = await GetEmployeeWithAllAvailableDepartmentsAsync();
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Create(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

                await _employeeService.InsertEmployeeAsync(employee);
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = employee.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = await GetAllAvailableDepartmentsAsync(model.DepartmentId);
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(Index), new { area = "" });
            }

            var model = employee.ToModel();
            model.AvailableDepartments = await GetAllAvailableDepartmentsAsync(employee.DepartmentId);
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = await _employeeService.GetByIdAsync(model.Id);
                if (employee != null)
                {
                    employee = model.ToEntity(employee);
                }

                await _employeeService.UpdateEmployeeAsync(employee);
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = employee.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = await GetAllAvailableDepartmentsAsync(model.DepartmentId);
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> GetFullSizeEmployeePicture(int id)
        {
            if (id > 0)
            {
                var pic = await _pictureService.GetPictureByIdAsync(id);
                var binary = await _pictureService.GetPictureBinaryByPictureIdAsync(id);
                return File(binary.BinaryData, pic.MimeType);
            }
            return NotFound();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> ListData(DepartmentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var departmentDict = (await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true)).ToDictionary(x => x.Id, x => x);
            var employees = await _employeeService.GetOrderedEmployeesAsync(showUnpublished: true, searchModel.Page - 1, searchModel.PageSize);
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
