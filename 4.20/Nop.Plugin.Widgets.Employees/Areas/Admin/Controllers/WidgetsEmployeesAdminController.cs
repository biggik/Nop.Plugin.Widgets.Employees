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
using Nop.Web.Areas.Admin.Controllers;

namespace Nop.Plugin.Widgets.Employees.Areas.Admin.Controllers
{
    public class WidgetsEmployeesController : BaseAdminController
    {
        const string AdminRoute = "~/Plugins/Widgets.Employees/Admin/Views/Employees/";

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

        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new EmployeeModel();

            return View($"{AdminRoute}Configure.cshtml", model);
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

            return View($"{AdminRoute}List.cshtml", model);
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
            Domain.Employee e;
            if (int.TryParse(id, out int employeeId))
            {
                e = _employeeService.GetById(employeeId);
            }
            else
            {
                e = _employeeService.GetByEmailPrefix(id);
            }
            var model = new EmployeeInfoModel
            {
                IsAdmin = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                Employee = e.ToModel()
            };
            model.Employee.PhotoUrl = GetPictureUrl(e.PictureId);
            model.Employee.DepartmentName = _employeeService.GetDepartmentById(e.DepartmentId)?.Name;

            return View($"{AdminRoute}EmployeeInfo.cshtml", model);
        }

        [HttpPost]
        public IActionResult EmployeeDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sbw = _employeeService.GetById(id);
            if (sbw != null)
                _employeeService.DeleteEmployee(sbw);

            return new NullJsonResult();
        }

        private IList<SelectListItem> GetAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return (from d in _employeeService.GetAllDepartments()
             select new SelectListItem
             {
                 Text = d.Name,
                 Value = d.Id.ToString(),
                 Selected = d.Id == selectedDepartmentId
             }).ToList();
        }

        private EmployeeModel GetEmployeeWithAllAvailableDepartments(int selectedDepartmentId = -1)
        {
            return new EmployeeModel
            {
                AvailableDepartments = GetAllAvailableDepartments(selectedDepartmentId)
            };
        }

        //add
        public IActionResult AddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var model = GetEmployeeWithAllAvailableDepartments(); 
            return View($"{AdminRoute}AddPopup.cshtml", model);
        }

        [HttpPost]
        public IActionResult AddPopup(string btnId, string formId, EmployeeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            _employeeService.InsertEmployee(model.ToEntity());

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View($"{AdminRoute}AddPopup.cshtml", model);
        }

        //edit
        public IActionResult EditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            var employee = _employeeService.GetById(id);
            if (employee == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            var model = GetEmployeeWithAllAvailableDepartments(employee.DepartmentId); 
            return View($"{AdminRoute}EditPopup.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditPopup(string btnId, string formId, EmployeeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var employee = _employeeService.GetById(model.Id);
            if (employee == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            employee = model.ToEntity();
            _employeeService.UpdateEmployee(employee);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View($"{AdminRoute}EditPopup.cshtml", model);
        }

        #region admin area

        public IActionResult CreateEmployee()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = GetEmployeeWithAllAvailableDepartments();
            return View($"{AdminRoute}CreateEmployee.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult CreateEmployee(EmployeeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

                _employeeService.InsertEmployee(employee);
                return continueEditing ? RedirectToAction("EditEmployee", new { id = employee.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
            return View($"{AdminRoute}CreateEmployee.cshtml", model);
        }

        public IActionResult EditEmployee(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var employee = _employeeService.GetById(id);
            var model = employee.ToModel();
            model.AvailableDepartments = GetAllAvailableDepartments(employee.DepartmentId);
            return View($"{AdminRoute}EditEmployee.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult EditEmployee(EmployeeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = _employeeService.GetById(model.Id);
                if (employee != null)
                {
                    employee = model.ToEntity(employee);
                }

                _employeeService.UpdateEmployee(employee);
                return continueEditing ? RedirectToAction("EditEmployee", new { id = employee.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
            return View($"{AdminRoute}EditEmployee.cshtml", model);
        }

        public IActionResult CreateDepartment()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = new DepartmentModel();
            return View($"{AdminRoute}CreateDepartment.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult CreateDepartment(DepartmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = model.ToEntity();
                _employeeService.InsertDepartment(department);
                return continueEditing ? RedirectToAction("EditDepartment", new { id = department.Id }) : RedirectToAction("DepartmentList");
            }

            //If we got this far, something failed, redisplay form
            return View($"{AdminRoute}CreateDepartment.cshtml", model);
        }

        public IActionResult EditDepartment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var department = _employeeService.GetDepartmentById(id);
            var model = department.ToModel();
            return View($"{AdminRoute}EditDepartment.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult EditDepartment(DepartmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = _employeeService.GetDepartmentById(model.Id);
                if(department != null)
                {
                    department = model.ToEntity(department);
                }

                _employeeService.UpdateDepartment(department);
                return continueEditing 
                    ? RedirectToAction("EditDepartment", new { id = department.Id }) 
                    : RedirectToAction("DepartmentList");
            }

            //If we got this far, something failed, redisplay form
            return View($"{AdminRoute}CreateDepartment.cshtml", model);
        }

        public IActionResult DepartmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = (from department in _employeeService.GetAllDepartments(true)
                         select department.ToModel()).ToList();

            return View($"{AdminRoute}DepartmentList.cshtml", model);
        }
        #endregion
    }
}
