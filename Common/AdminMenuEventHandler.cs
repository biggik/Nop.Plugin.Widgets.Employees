using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Employees.Controllers;
using Nop.Plugin.Widgets.Employees.Extensions;
using Nop.Plugin.Widgets.Employees.Resources;
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
                Title = EmployeeResources.ListCaption,
                Visible = true,
                IconClass = "fa-cubes"
            };
            rootNode.ChildNodes.Add(contentMenu);
        }

        // Add child menu items
        async Task<string> TAsync(string format) => await _localizationService.GetResourceAsync(format) ?? format;

        foreach (var item in new List<(string caption, string controller, string action)>
            {
                (await TAsync(EmployeeResources.ListCaption), EmployeesController.ControllerName, nameof(EmployeesController.ListAsync).NoAsync()),
                (await TAsync(DepartmentResources.ListCaption), DepartmentsController.ControllerName, nameof(DepartmentsController.ListAsync).NoAsync())
            })
        {
            contentMenu.ChildNodes.Add(new AdminMenuItem
            {
                SystemName = $"{item.controller}.{item.action}",
                Title = item.caption,
                Url = $"/Admin/{item.controller}/{item.action}",
                Visible = true,
                IconClass = "far fa-dot-circle"
            });
        }
    }
}
