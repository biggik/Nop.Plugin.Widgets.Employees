using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Employees.Components
{
    [ViewComponent(Name = "WidgetsEmployees")]
    public class WidgetsEmployeesViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Widgets.Employees/Views/Employees/PublicInfo.cshtml");
        }
    }
}
