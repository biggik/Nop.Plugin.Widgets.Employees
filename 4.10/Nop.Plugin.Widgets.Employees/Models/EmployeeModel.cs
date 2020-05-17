using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.Employees.Models
{
    public class EmployeeModel : BaseNopEntityModel
    {
        public EmployeeModel()
        {
            AvailableDepartments = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Department")]
        public int DepartmentId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.PictureId")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Info")]
        public string Info { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Specialties")]
        public string Specialties { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Interests")]
        public string Interests { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.MobileNumber")]
        public string MobileNumber { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.WorkStarted")]
        public DateTime WorkStarted { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.WorkEnded")]
        public DateTime WorkEnded { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.Deleted")]
        public bool Deleted { get; set; }

        public IList<SelectListItem> AvailableDepartments { get; set; }

        public string PhotoUrl { get; set; }
        public string DepartmentName { get; set; }
    }
}