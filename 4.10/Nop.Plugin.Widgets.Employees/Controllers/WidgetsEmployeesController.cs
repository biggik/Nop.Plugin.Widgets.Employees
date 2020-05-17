using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Security;
using Nop.Core;
using Nop.Services.Media;

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
            this._employeeSettings = employeeSettings;
            this._employeeService = employeeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._pictureService = pictureService;
        }

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    CommonHelper.SetTelerikCulture();

        //    base.Initialize(requestContext);
        //}
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new EmployeeModel();
            //other settings

            return View("~/Plugins/Widgets.Employees/Views/Employees/Configure.cshtml", model);
        }
        

        [HttpPost]
        public IActionResult EmployeeList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            var records = _employeeService.GetAll(command.Page - 1, command.PageSize);
            var sbwModel = records.Select(x =>
                {
                    return x.ToModel();
                })
                .ToList();
            var model = new DataSourceResult
            {
                Data = sbwModel,
                Total = records.TotalCount
            };

            return Json(model);
        }

        [HttpPost]
        public IActionResult DepartmentList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var records = _employeeService.GetAllDepartments(true);
            var sbwModel = records.Select(x => x.ToModel()).ToList();
            var model = new DataSourceResult
            {
                Data = sbwModel,
                Total = records.Count
            };

            return Json(model);
        }

        public IActionResult EmployeeInfo(int id)
        {
            var model = new EmployeeInfoModel
            {
                IsAdmin = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel)
            };
            var e = _employeeService.GetById(id);
            model.Employee = e.ToModel();
            if (e.PictureId > 0)
                model.Employee.PhotoUrl = _pictureService.GetPictureUrl(e.PictureId, 200);
            else
                model.Employee.PhotoUrl = null;
            var department = _employeeService.GetDepartmentByDepartmentId(e.DepartmentId);
            if (department != null)
            {
                model.Employee.DepartmentName = department.Name;
            }
            return View("~/Plugins/Widgets.Employees/Views/Employees/EmployeeInfo.cshtml", model);

        }

        [HttpPost]
        public IActionResult EmployeeDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sbw = _employeeService.GetById(id);
            if (sbw != null)
                _employeeService.DeleteEmployeesRecord(sbw);

            return new NullJsonResult();
        }

        //add
        public IActionResult AddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var model = new EmployeeModel();

            foreach (var c in _employeeService.GetAllDepartments())
                model.AvailableDepartments.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.DepartmentId) });

            return View("~/Plugins/Widgets.Employees/Views/Employees/AddPopup.cshtml", model);
        }

        [HttpPost]
        public IActionResult AddPopup(string btnId, string formId, EmployeeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var employee = model.ToEntity();
            _employeeService.InsertEmployeesRecord(employee);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View("~/Plugins/Widgets.Employees/Views/Employees/AddPopup.cshtml", model);
        }

        //edit
        public IActionResult EditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            var sbw = _employeeService.GetById(id);
            if (sbw == null)
                //No record found with the specified id
                return RedirectToAction("Configure");

            var model = sbw.ToModel();
            
            foreach (var c in _employeeService.GetAllDepartments())
                model.AvailableDepartments.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.DepartmentId) });

            return View("~/Plugins/Widgets.Employees/Views/Employees/EditPopup.cshtml", model);
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
            _employeeService.UpdateEmployeesRecord(employee);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View("~/Plugins/Widgets.Employees/Views/Employees/EditPopup.cshtml", model);
        }

        //public IActionResult PublicInfo()
        //{
        //    return View("~/Plugins/Widgets.Employees/Views/Employees/PublicInfo.cshtml");
        //}

        public IActionResult List()
        {
            var model = new DepartmentEmployeeModel
            {
                IsAdmin = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel)
            };

            foreach (var d in _employeeService.GetAllDepartments())
            {
                var employeesModel = new EmployeesListModel {DepartmentName = d.Name};
                foreach (var e in _employeeService.GetEmployeeByDepartmentId(d.Id))
                {
                    employeesModel.Employees.Add(e.ToModel());
                }
                model.EmployeesList.Add(employeesModel);
            }

            return View("~/Plugins/Widgets.Employees/Views/Employees/List.cshtml", model);
        }

        #region admin area

        public IActionResult CreateEmployee()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = new EmployeeModel();
            var departments = _employeeService.GetAllDepartments();
            foreach (var c in departments)
                model.AvailableDepartments.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.DepartmentId) });
            return View("~/Plugins/Widgets.Employees/Views/Employees/CreateEmployee.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult CreateEmployee(EmployeeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

                _employeeService.InsertEmployeesRecord(employee);
                return continueEditing ? RedirectToAction("EditEmployee", new { id = employee.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            var departments = _employeeService.GetAllDepartments();
            foreach (var c in departments)
                model.AvailableDepartments.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.DepartmentId) });
            return View("~/Plugins/Widgets.Employees/Views/Employees/CreateEmployee.cshtml", model);
        }

        public IActionResult EditEmployee(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var employee = _employeeService.GetById(id);
            var model = employee.ToModel();
            foreach (var c in _employeeService.GetAllDepartments())
                model.AvailableDepartments.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.DepartmentId) });
            return View("~/Plugins/Widgets.Employees/Views/Employees/EditEmployee.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
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

                _employeeService.UpdateEmployeesRecord(employee);
                return continueEditing ? RedirectToAction("EditEmployee", new { id = employee.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            foreach (var c in _employeeService.GetAllDepartments())
                model.AvailableDepartments.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.DepartmentId) });
            return View("~/Plugins/Widgets.Employees/Views/Employees/EditEmployee.cshtml", model);
        }

        public IActionResult CreateDepartment()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = new DepartmentModel();
            return View("~/Plugins/Widgets.Employees/Views/Employees/CreateDepartment.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult CreateDepartment(DepartmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = model.ToEntity();
                _employeeService.InsertDepartmentRecord(department);
                return continueEditing ? RedirectToAction("EditDepartment", new { id = department.Id }) : RedirectToAction("DepartmentList");
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Widgets.Employees/Views/Employees/CreateDepartment.cshtml", model);
        }

        public IActionResult EditDepartment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var department = _employeeService.GetDepartmentByDepartmentId(id);
            var model = department.ToModel();
            return View("~/Plugins/Widgets.Employees/Views/Employees/EditDepartment.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult EditDepartment(DepartmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = _employeeService.GetDepartmentByDepartmentId(model.Id);
                if(department != null)
                {
                    department = model.ToEntity(department);
                }

                _employeeService.UpdateDepartmentRecord(department);
                return continueEditing 
                    ? RedirectToAction("EditDepartment", new { id = department.Id }) 
                    : RedirectToAction("DepartmentList");
            }

            //If we got this far, something failed, redisplay form
            return View("~/Plugins/Widgets.Employees/Views/Employees/CreateDepartment.cshtml", model);
        }


        public IActionResult DepartmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = new List<DepartmentModel>();
            foreach (var item in _employeeService.GetAllDepartments(true))
            {
                model.Add(item.ToModel());
            }

            return View("~/Plugins/Widgets.Employees/Views/Employees/DepartmentList.cshtml", model);
        }
        #endregion

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected IActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            //return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
            return Content("Access denied");
        }
    }
}
