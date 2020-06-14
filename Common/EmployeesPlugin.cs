using System.Linq;
using System.Collections.Generic;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.Widgets.Employees.Data;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Localization;
using Nop.Core;
using Nop.Services.Plugins;
using Nop.Plugin.Widgets.Employees.Resources;
using Nop.Web.Framework.Menu;
using System;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Security;
using Nop.Plugin.Widgets.Employees.Controllers;
using nopLocalizationHelper;

namespace Nop.Plugin.Widgets.Employees
{
    public class EmployeesPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
#if NOP_PRE_4_3
        private readonly EmployeesObjectContext _objectContext;
#endif
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;

        public bool HideInWidgetList => false;

        public EmployeesPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IPermissionService permissionService,
#if NOP_PRE_4_3
            EmployeesObjectContext objectContext,
#endif
            IStoreContext storeContext,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _languageService = languageService;
            _permissionService = permissionService;
#if NOP_PRE_4_3
            _objectContext = objectContext;
#endif
            _settingService = settingService;
#if DEBUG
            CreateLocaleStrings();
#endif

            var storeScope = storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<EmployeeWidgetSettings>(storeScope);

            _widgetZones = string.IsNullOrWhiteSpace(settings.WidgetZones)
                ? new List<string>()
                : settings.WidgetZones.Split(';').ToList();
        }

        private LocaleStringHelper<LocaleStringResource> ResourceHelper()
        {
            return new LocaleStringHelper<LocaleStringResource>
            (
                GetType().Assembly,
                from lang in _languageService.GetAllLanguages() select (lang.Id, lang.LanguageCulture),
                (resourceName, languageId) => _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false),
                (languageId, resourceName, resourceValue) => new LocaleStringResource { LanguageId = languageId, ResourceName = resourceName, ResourceValue = resourceValue },
                (lsr) => _localizationService.InsertLocaleStringResource(lsr),
                (lsr, resourceValue) => { lsr.ResourceValue = resourceValue; _localizationService.UpdateLocaleStringResource(lsr); },
                (lsr) => _localizationService.DeleteLocaleStringResource(lsr),
                (lsr, resourceValue) => lsr.ResourceValue == resourceValue
            );
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones() => _widgetZones;
        List<string> _widgetZones;

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl() => $"{_webHelper.GetStoreLocation()}Admin/Employees/Configure";

        private void CreateLocaleStrings()
        {
            ResourceHelper().CreateLocaleStrings();
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _settingService.SaveSetting(new EmployeeWidgetSettings { });

#if NOP_PRE_4_3
            _objectContext.Install();
#endif

            CreateLocaleStrings();
            _permissionService.InstallPermissions(new EmployeePermissionProvider());

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<EmployeeWidgetSettings>();

#if NOP_PRE_4_3
            _objectContext.Uninstall();
#endif

            ResourceHelper().DeleteLocaleStrings();

            _permissionService.UninstallPermissions(new EmployeePermissionProvider());

            base.Uninstall();
        }

        public string GetWidgetViewComponentName(string widgetZone) => "WidgetsEmployees";

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var contentMenu = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Content Management");
            if (contentMenu == null)
            {
                // Unable to find the "Configure" menu, create our own menu container
                contentMenu = new SiteMapNode()
                {
                    SystemName = "Employees",
                    Title = EmployeeResources.ListCaption,
                    Visible = true,
                    IconClass = "fa-cubes"
                };
                rootNode.ChildNodes.Add(contentMenu);
            }

            string T(string format) => _localizationService.GetResource(format) ?? format;
            
            foreach (var item in new List<(string caption, string controller, string action)>
            {
                (T(EmployeeResources.ListCaption), EmployeesController.ControllerName, nameof(EmployeesController.List)),
                (T(AdminResources.DepartmentListCaption), DepartmentsController.ControllerName, nameof(DepartmentsController.List)),
            })
            {
                contentMenu.ChildNodes.Add(new SiteMapNode
                {
                    SystemName = $"{item.controller}.{item.action}",
                    Title = item.caption,
                    ControllerName = item.controller,
                    ActionName = item.action,
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary {
                    { "area", "Admin" }
                },
                });
            }
        }
    }
}
