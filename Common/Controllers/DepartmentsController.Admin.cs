using Nop.Plugin.Widgets.Employees.Models;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Core;
using Nop.Services.Media;
using Nop.Services.Messages;
using System.Linq;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Controllers;
using System.Threading.Tasks;

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
        [Area(AreaNames.Admin)]
#if NOP_ASYNC
        public async Task<IActionResult> Create()
#else
        public IActionResult Create()
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

            var model = new DepartmentModel();
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
#if NOP_ASYNC
        public async Task<IActionResult> Create(DepartmentModel model, bool continueEditing)
#else
        public IActionResult Create(DepartmentModel model, bool continueEditing)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = model.ToEntity();
#if NOP_ASYNC
                await _employeeService.InsertDepartmentAsync(department);
#else
                _employeeService.InsertDepartment(department);
#endif
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = department.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
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
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

#if NOP_ASYNC
            var department = await _employeeService.GetDepartmentByIdAsync(id);
#else
            var department = _employeeService.GetDepartmentById(id);
#endif
            var model = department.ToModel();
            return View($"{Route}{nameof(Edit)}.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
#if NOP_ASYNC
        public async Task<IActionResult> Edit(DepartmentModel model, bool continueEditing)
#else
        public IActionResult Edit(DepartmentModel model, bool continueEditing)
#endif
        {
#if NOP_ASYNC
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
#if NOP_ASYNC
                var department = await _employeeService.GetDepartmentByIdAsync(model.Id);
#else
                var department = _employeeService.GetDepartmentById(model.Id);
#endif
                if (department != null)
                {
                    department = model.ToEntity(department);
                }

#if NOP_ASYNC
                await _employeeService.UpdateDepartmentAsync(department);
#else
                _employeeService.UpdateDepartment(department);
#endif
                return continueEditing
                    ? RedirectToAction(nameof(Edit), new { id = department.Id })
                    : RedirectToAction(nameof(List));
            }

            //If we got this far, something failed, redisplay form
            return View($"{Route}{nameof(Create)}.cshtml", model);
        }

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
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

#if NOP_ASYNC
            var department = await _employeeService.GetDepartmentByIdAsync(id);
            if (department != null)
                await _employeeService.DeleteDepartmentAsync(department);
#else
            var department = _employeeService.GetDepartmentById(id);
            if (department != null)
                _employeeService.DeleteDepartment(department);
#endif

            return RedirectToAction(nameof(List), new { area = "Admin" });
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
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

            var model = new DepartmentSearchModel();
            model.SetGridPageSize();
            return View($"{Route}{nameof(List)}.cshtml", model);
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
            if (!await _permissionService.AuthorizeAsync(EmployeePermissionProvider.ManageDepartments))
#else
            if (!_permissionService.Authorize(EmployeePermissionProvider.ManageDepartments))
#endif
                return AccessDeniedView();

#if NOP_ASYNC
            var departments = await _employeeService.GetOrderedDepartmentsAsync(showUnpublished: true, searchModel.Page - 1, searchModel.PageSize);
#else
            var departments = _employeeService.GetOrderedDepartments(showUnpublished:true, searchModel.Page - 1, searchModel.PageSize);
#endif
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
