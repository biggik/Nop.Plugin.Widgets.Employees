using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Employees.Constants;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class DepartmentsController : BasePluginController
    {
        public static string ControllerName = nameof(DepartmentsController).Replace("Controller", "");
        const string Route = "~/Plugins/Widgets.Employees/Views/Departments/";

        private readonly IEmployeesService _employeeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;

        public DepartmentsController(
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

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
#if NOP_48
        [CheckPermission(EmployeePermissionConfigs.MANAGE_DEPARTMENTS)]
#endif
        public async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            var model = new DepartmentModel();
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Create(DepartmentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = model.ToEntity();
                await _employeeService.InsertDepartmentAsync(department);
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = department.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            var department = await _employeeService.GetDepartmentByIdAsync(id);
            var model = department.ToModel();
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(DepartmentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = await _employeeService.GetDepartmentByIdAsync(model.Id);
                if (department != null)
                {
                    department = model.ToEntity(department);
                }

                await _employeeService.UpdateDepartmentAsync(department);
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = department.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            var department = await _employeeService.GetDepartmentByIdAsync(id);
            if (department != null)
                await _employeeService.DeleteDepartmentAsync(department);

            return RedirectToAction(nameof(List), new { area = "Admin" });
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            var model = new DepartmentSearchModel();
            model.SetGridPageSize();
            return View($"{Route}{nameof(List)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(Areas.Admin)]
        public async Task<IActionResult> ListData(DepartmentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(
#if NOP_47
                EmployeePermissionProvider.ManageDepartments))
#else
                EmployeePermissionConfigs.MANAGE_DEPARTMENTS))
#endif
                return AccessDeniedView();

            var departments = await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true, searchModel.Page - 1, searchModel.PageSize);
            var model = new DepartmentListModel().PrepareToGrid(searchModel, departments, () =>
            {
                return departments.Select(department =>
                {
                    var d = department.ToModel();
                    return d;
                });
            });

            return Json(model);
        }
    }
}
