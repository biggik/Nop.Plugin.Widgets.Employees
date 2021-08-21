using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;
using Nop.Plugin.Widgets.Employees.Resources;

namespace Nop.Plugin.Widgets.Employees.Models
{
#if NOP_ASYNC
    public record
#else
    public class 
#endif
        DepartmentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName(DepartmentResources.Name)]
        public string Name { get; set; }

        [NopResourceDisplayName(DepartmentResources.PictureId)]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName(GenericResources.DisplayOrder)]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName(GenericResources.Published)]
        public bool Published { get; set; }

        public string PictureUrl { get; set; }
    }
}