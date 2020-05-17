using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.Employees.Models
{
    public class DepartmentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.DepartmentName")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.DepartmentPictureId")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.DepartmentDisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Employees.Fields.DepartmentPublished")]
        public bool Published { get; set; }
    }
}