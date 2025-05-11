using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.Employees.Constants;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class EmployeesController
    {
        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> ConfigureAsync()
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
            {
                return AccessDeniedView();
            }
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.value, y => y.id);

            var currentWidgetZones = await (from i in (settings.WidgetZones ?? "").Split(';')
                                            where lookup.ContainsKey(i)
                                            select lookup[i]).ToListAsync();

            var model = new ConfigurationModel
            {
                WidgetZones = currentWidgetZones,
                AvailableWidgetZones = await (from wzd in widgetZonesData
                                              select new SelectListItem
                                              {
                                                  Text = wzd.name,
                                                  Value = wzd.id.ToString(),
                                                  Selected = currentWidgetZones.Contains(wzd.id)
                                              }
                                       ).ToListAsync()
            };

            return View($"{Route}{nameof(ConfigureAsync)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
            {
                return AccessDeniedView();
            }

            if (!ModelState.IsValid)
                return await ConfigureAsync();

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

            return await ConfigureAsync();
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> ListAsync()
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
                return AccessDeniedView();

            var model = new EmployeeSearchModel();
            model.SetGridPageSize();
            return View($"{Route}{nameof(ListAsync)}.cshtml", model);
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
            return await (from d in (await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true))
                    .Where(dep => dep.Id == selectedDepartmentId || dep.Published)
                          select new SelectListItem
                          {
                              Text = d.Name,
                              Value = d.Id.ToString(),
                              Selected = d.Id == selectedDepartmentId
                          })
                    .ToListAsync();
        }

        private async Task<EmployeeModel> GetEmployeeWithAllAvailableDepartmentsAsync(int selectedDepartmentId = -1)
        {
            return new EmployeeModel
            {
                AvailableDepartments = await GetAllAvailableDepartmentsAsync(selectedDepartmentId)
            };
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
                return AccessDeniedView();

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee != null)
                await _employeeService.DeleteEmployeeAsync(employee);

            return RedirectToAction(nameof(IndexAsync), new { area = "" });
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> CreateAsync()
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
                return AccessDeniedView();

            var model = await GetEmployeeWithAllAvailableDepartmentsAsync();
            return View($"{Route}{nameof(CreateAsync)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> CreateAsync(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

                await _employeeService.InsertEmployeeAsync(employee);
                return continueEditing
                    ? RedirectToAction(nameof(EditAsync), new { id = employee.Id })
                    : RedirectToAction(nameof(ListAsync));
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = await GetAllAvailableDepartmentsAsync(model.DepartmentId);
            return View($"{Route}{nameof(CreateAsync)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> EditAsync(int id)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
                return AccessDeniedView();

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(IndexAsync), new { area = "" });
            }

            var model = employee.ToModel();
            model.AvailableDepartments = await GetAllAvailableDepartmentsAsync(employee.DepartmentId);
            return View($"{Route}{nameof(EditAsync)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> EditAsync(EmployeeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
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
                    ? RedirectToAction(nameof(EditAsync), new { id = employee.Id })
                    : RedirectToAction(nameof(ListAsync));
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = await GetAllAvailableDepartmentsAsync(model.DepartmentId);
            return View($"{Route}{nameof(EditAsync)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> GetFullSizeEmployeePictureAsync(int id)
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
        [Area(Areas.Admin)]
        public async Task<IActionResult> ListDataAsync(DepartmentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageEmployees))
#else
                EmployeePermissionConfigs.MANAGE_EMPLOYEES))
#endif
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
