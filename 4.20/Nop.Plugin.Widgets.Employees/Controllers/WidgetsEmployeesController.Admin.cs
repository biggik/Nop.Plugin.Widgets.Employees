using System.Linq;
using Nop.Plugin.Widgets.Employees.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace Nop.Plugin.Widgets.Employees.Controllers
{
    public partial class WidgetsEmployeesController 
    {
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new EmployeeModel();

            return View($"{Route}Configure.cshtml", model);
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

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
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
                    ? RedirectToAction(nameof(EditEmployee), new { id = employee.Id })
                    : RedirectToAction(nameof(WidgetsEmployeesController.Index), WidgetsEmployeesController.ControllerName);
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
                    ? RedirectToAction(nameof(EditEmployee), new { id = employee.Id })
                    : RedirectToAction(nameof(WidgetsEmployeesController.Index), WidgetsEmployeesController.ControllerName);
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

            var model = (from department in _employeeService.GetAllDepartments(true)
                         select department.ToModel()).ToList();

            return View($"{Route}DepartmentList.cshtml", model);
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
