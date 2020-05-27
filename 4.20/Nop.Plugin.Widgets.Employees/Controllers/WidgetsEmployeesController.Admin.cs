using System.Linq;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class WidgetsEmployeesController 
    {
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            return View($"{Route}Configure.cshtml", new EmployeeModel());
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
        public IActionResult EmployeeDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var employee = _employeeService.GetById(id);
            if (employee != null)
                _employeeService.DeleteEmployee(employee);

            return RedirectToAction(nameof(Index), new { area = "" });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult CreateEmployee()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = GetEmployeeWithAllAvailableDepartments();
            return View($"{Route}CreateEmployee.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult CreateEmployee(EmployeeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var employee = model.ToEntity();

                _employeeService.InsertEmployee(employee);
                return continueEditing
                    ? RedirectToAction(nameof(EmployeeInfo), new { id = employee.Id, area = "" })
                    : RedirectToAction(nameof(Index), ControllerName, new { area = "" });
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
            return View($"{Route}CreateEmployee.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult EditEmployee(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(Index), new { area = "" });
            }

            var model = employee.ToModel();
            model.AvailableDepartments = GetAllAvailableDepartments(employee.DepartmentId);
            return View($"{Route}EditEmployee.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
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
                return continueEditing
                    ? RedirectToAction(nameof(EmployeeInfo), new { id = employee.Id, area = "" })
                    : RedirectToAction(nameof(Index), ControllerName, new { area = "" });
            }

            //If we got this far, something failed, redisplay form
            model.AvailableDepartments = GetAllAvailableDepartments(model.DepartmentId);
            return View($"{Route}EditEmployee.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult CreateDepartment()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = new DepartmentModel();
            return View($"{Route}CreateDepartment.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult CreateDepartment(DepartmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = model.ToEntity();
                _employeeService.InsertDepartment(department);
                return continueEditing
                    ? RedirectToAction(nameof(EditDepartment), new { id = department.Id })
                    : RedirectToAction(nameof(DepartmentList));
            }

            //If we got this far, something failed, redisplay form
            return View($"{Route}CreateDepartment.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult EditDepartment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var department = _employeeService.GetDepartmentById(id);
            var model = department.ToModel();
            return View($"{Route}EditDepartment.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public IActionResult EditDepartment(DepartmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var department = _employeeService.GetDepartmentById(model.Id);
                if (department != null)
                {
                    department = model.ToEntity(department);
                }

                _employeeService.UpdateDepartment(department);
                return continueEditing
                    ? RedirectToAction(nameof(EditDepartment), new { id = department.Id })
                    : RedirectToAction(nameof(DepartmentList));
            }

            //If we got this far, something failed, redisplay form
            return View($"{Route}CreateDepartment.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult DepartmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            return View($"{Route}DepartmentList.cshtml", new DepartmentSearchModel());
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult DepartmentListData(DepartmentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            var departments = _employeeService.GetAllDepartments(showUnpublished:true, searchModel.Page - 1, searchModel.PageSize);
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

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult GetFullSizeEmployeePicture(int id)
        {
            if (id > 0)
            {
                var pic = _pictureService.GetPictureById(id);
                return File(pic.PictureBinary.BinaryData, pic.MimeType);
            }
            return NotFound();
        }
    }
}
