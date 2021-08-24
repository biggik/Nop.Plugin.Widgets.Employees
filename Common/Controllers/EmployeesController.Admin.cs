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
#if NOP_ASYNC
        public async Task<IActionResult> Configure()
#else
        public IActionResult Configure()
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
            {
                return AccessDeniedView();
            }
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
            {
                return AccessDeniedView();
            }
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<EmployeeWidgetSettings>(storeScope);
#endif

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
#if NOP_ASYNC
        public async Task<IActionResult> Configure(ConfigurationModel model)
#else
        public IActionResult Configure(ConfigurationModel model)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
            {
                return AccessDeniedView();
            }

            if (!ModelState.IsValid)
#if NOP_ASYNC
                return await Configure();
#else
                return Configure();
#endif

#if NOP_ASYNC
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);
#else
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<EmployeeWidgetSettings>(storeScope);
#endif

            var widgetZonesData = GetWidgetZoneData();
            var lookup = widgetZonesData.ToDictionary(x => x.id, y => y.value);

            settings.WidgetZones = model.WidgetZones != null && model.WidgetZones.Any()
                ? string.Join(";",
                        from i in model.WidgetZones
                        where lookup.ContainsKey(i)
                        select lookup[i]
                        )
                : "";

#if NOP_ASYNC
            await _settingService.SaveSettingAsync(settings, s => s.WidgetZones, clearCache: false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
#else
            _settingService.SaveSetting(settings, s => s.WidgetZones, clearCache: false);
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
#endif

#if NOP_ASYNC
            return await Configure();
#else
            return Configure();
#endif
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
#if NOP_ASYNC
        public async Task<IActionResult> List()
#else
        public IActionResult List()
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
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

#if NOP_ASYNC
        private async Task<IList<SelectListItem>> GetAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return (from d in (await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true))
#else
        private IList<SelectListItem> GetAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return (from d in _employeeService.GetOrderedDepartments(showUnpublished: true)
#endif
                    .Where(dep => dep.Id == selectedDepartmentId || dep.Published)
                    select new SelectListItem
                    {
                        Text = d.Name,
                        Value = d.Id.ToString(),
                        Selected = d.Id == selectedDepartmentId
                    })
                    .ToList();
        }

#if NOP_ASYNC
        private async Task<EmployeeModel> GetEmployeeWithAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return new EmployeeModel
            {
                AvailableDepartments = await GetAllAvailableDepartments(selectedDepartmentId)
            };
        }
#else
        private EmployeeModel GetEmployeeWithAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return new EmployeeModel
            {
                AvailableDepartments = GetAllAvailableDepartments(selectedDepartmentId)
            };
        }
#endif

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
#if NOP_ASYNC
        public async Task<IActionResult> Delete(int id)
#else
        public IActionResult Delete(int id)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee != null)
                await _employeeService.DeleteEmployeeAsync(employee);
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
                return AccessDeniedView();

            var employee = _employeeService.GetById(id);
            if (employee != null)
                _employeeService.DeleteEmployee(employee);
#endif

            return RedirectToAction(nameof(Index), new { area = "" });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
#if NOP_ASYNC
        public async Task<IActionResult> Create()
#else
        public IActionResult Create()
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
                return AccessDeniedView();

            var model = GetEmployeeWithAllAvailableDepartments();
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
#if NOP_ASYNC
        public async Task<IActionResult> Create(EmployeeModel model, bool continueEditing)
#else
        public IActionResult Create(EmployeeModel model, bool continueEditing)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

#if NOP_ASYNC
                await _employeeService.InsertEmployeeAsync(employee);
#else
                _employeeService.InsertEmployee(employee);
#endif
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = employee.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
#if NOP_ASYNC
            model.AvailableDepartments = await GetAllAvailableDepartments(model.DepartmentId);
#else
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
#endif
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
#if NOP_ASYNC
        public async Task<IActionResult> Edit(int id)
#else
        public IActionResult Edit(int id)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
                return AccessDeniedView();

#if NOP_ASYNC
            var employee = await _employeeService.GetByIdAsync(id);
#else
            var employee = _employeeService.GetById(id);
#endif
            if (employee == null)
            {
                return RedirectToAction(nameof(Index), new { area = "" });
            }

            var model = employee.ToModel();
#if NOP_ASYNC
            model.AvailableDepartments = await GetAllAvailableDepartments(employee.DepartmentId);
#else
            model.AvailableDepartments = GetAllAvailableDepartments(employee.DepartmentId);
#endif
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
#if NOP_ASYNC
        public async Task<IActionResult> Edit(EmployeeModel model, bool continueEditing)
#else
        public IActionResult Edit(EmployeeModel model, bool continueEditing)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
#if NOP_ASYNC
                var employee = await _employeeService.GetByIdAsync(model.Id);
#else
                var employee = _employeeService.GetById(model.Id);
#endif
                if (employee != null)
                {
                    employee = model.ToEntity(employee);
                }

#if NOP_ASYNC
                await _employeeService.UpdateEmployeeAsync(employee);
#else
                _employeeService.UpdateEmployee(employee);
#endif
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = employee.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
#if NOP_ASYNC
            model.AvailableDepartments = await GetAllAvailableDepartments(model.DepartmentId);
#else
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
#endif
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
#if NOP_ASYNC
        public async Task<IActionResult> GetFullSizeEmployeePicture(int id)
#else
        public IActionResult GetFullSizeEmployeePicture(int id)
#endif
        {
            if (id > 0)
            {
#if NOP_ASYNC
                var pic = await _pictureService.GetPictureByIdAsync(id);
#else
                var pic = _pictureService.GetPictureById(id);
#endif
#if NOP_PRE_4_3
                return File(pic.PictureBinary.BinaryData, pic.MimeType);
#else
#if NOP_ASYNC
                var binary = await _pictureService.GetPictureBinaryByPictureIdAsync(id);
#else
                var binary = _pictureService.GetPictureBinaryByPictureId(id);
#endif
                return File(binary.BinaryData, pic.MimeType);
#endif
            }
            return NotFound();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
#if NOP_ASYNC
        public async Task<IActionResult> ListData(DepartmentSearchModel searchModel)
#else
        public IActionResult ListData(DepartmentSearchModel searchModel)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageEmployees))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageEmployees))
#endif
                return AccessDeniedView();

#if NOP_ASYNC
            var departmentDict = (await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true)).ToDictionary(x => x.Id, x => x);
            var employees = await _employeeService.GetOrderedEmployeesAsync(showUnpublished: true, searchModel.Page - 1, searchModel.PageSize);
#else
            var departmentDict = _employeeService.GetOrderedDepartments(showUnpublished: true).ToDictionary(x => x.Id, x => x);
            var employees = _employeeService.GetOrderedEmployees(showUnpublished: true, searchModel.Page - 1, searchModel.PageSize);
#endif
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
