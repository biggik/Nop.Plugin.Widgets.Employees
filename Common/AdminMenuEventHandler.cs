using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Widgets.Employees;

public class AdminMenuEventHandler : IConsumer<AdminMenuCreatedEvent>
{
    private readonly ILocalizationService _localizationService;

    public AdminMenuEventHandler(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var rootNode = eventMessage.RootMenuItem;

        // Find or create the "Content Management" menu
        var contentMenu = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Content Management");
        if (contentMenu == null)
        {
            contentMenu = new AdminMenuItem
            {
                SystemName = "Employees",
                Title = await _localizationService.GetResourceAsync("Admin.Employees.ListCaption") ?? "Employees",
                Visible = true,
                IconClass = "fa-cubes"
            };
            rootNode.ChildNodes.Add(contentMenu);
        }

        // Add child menu items
        contentMenu.ChildNodes.Add(new AdminMenuItem
        {
            SystemName = "Employees.List",
            Title = await _localizationService.GetResourceAsync("Admin.Employees.ListCaption") ?? "Employee List",
            Url = "/Admin/Employees/List",
            Visible = true,
            IconClass = "fa-dot-circle-o"
        });

        contentMenu.ChildNodes.Add(new AdminMenuItem
        {
            SystemName = "Departments.List",
            Title = await _localizationService.GetResourceAsync("Admin.Departments.ListCaption") ?? "Department List",
            Url = "/Admin/Departments/List",
            Visible = true,
            IconClass = "fa-dot-circle-o"
        });
    }
}
