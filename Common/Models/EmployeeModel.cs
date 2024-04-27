using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Plugin.Widgets.Employees.Resources;

namespace Nop.Plugin.Widgets.Employees.Models
{
    public record EmployeeModel : BaseNopEntityModel
    {
        public EmployeeModel()
        {
            AvailableDepartments = new List<SelectListItem>();
        }

        [NopResourceDisplayName(EmployeeResources.Department)]
        public int DepartmentId { get; set; }

        [NopResourceDisplayName(EmployeeResources.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(EmployeeResources.Title)]
        public string Title { get; set; }

        [NopResourceDisplayName(EmployeeResources.Email)]
        public string Email { get; set; }

        [NopResourceDisplayName(EmployeeResources.PictureId)]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName(EmployeeResources.Info)]
        public string Info { get; set; }

        [NopResourceDisplayName(EmployeeResources.Specialties)]
        public string Specialties { get; set; }

        [NopResourceDisplayName(EmployeeResources.Interests)]
        public string Interests { get; set; }

        [NopResourceDisplayName(EmployeeResources.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName(EmployeeResources.MobileNumber)]
        public string MobileNumber { get; set; }

        [NopResourceDisplayName(EmployeeResources.WorkStarted)]
        public DateTime WorkStarted { get; set; }

        [NopResourceDisplayName(EmployeeResources.WorkEnded)]
        [UIHint("DateTimeNullable")]
        public DateTime? WorkEnded { get; set; }

        [NopResourceDisplayName(GenericResources.Published)]
        public bool Published { get; set; }

        [NopResourceDisplayName(GenericResources.DisplayOrder)]
        public int DisplayOrder { get; set; }

        public string PhotoUrl { get; set; }
        public bool DepartmentPublished { get; set; }
        public string DepartmentName { get; set; }
        public bool HasUniqueEmail { get; set; }
        public string EmailPrefixOrId => (Email?.Contains('@') ?? false) && HasUniqueEmail ? Email.Split('@')[0] : Id.ToString();
     
        public IList<SelectListItem> AvailableDepartments { get; set; }

        public bool IsActive => Published
                               && DepartmentPublished
                               && (WorkStarted.Date <= DateTime.Now.Date)
                               && (!WorkEnded.HasValue ||
                                    (WorkEnded.Value.Year < 2000 || WorkEnded.Value > DateTime.Now.Date));
    }
}