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
using System.Threading.Tasks;
using System.Diagnostics;
using Nop.Plugin.Widgets.Employees.Components;

namespace Nop.Plugin.Widgets.Employees
{
    public class EmployeesPlugin : BasePlugin, IWidgetPlugin
#if NOP_47
        , IAdminMenuPlugin
#endif
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;

        public bool HideInWidgetList => false;

        public EmployeesPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _languageService = languageService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _settingService = settingService;
#if DEBUG
            Instance = this;
#endif
        }

#if DEBUG
        internal static EmployeesPlugin Instance { get; private set; }
        private static bool _debugInitialized = false;

        internal async Task VerifyLocaleResourcesAsync()
        {
            if (_debugInitialized)
                return;

            _debugInitialized = true;
            await (await ResourceHelperAsync()).CreateLocaleStringsAsync();
#if NOP_47
            var t = _permissionService.InstallPermissionsAsync(new EmployeePermissionProvider());
            t.Wait();
#endif
        }
#endif

        private async Task<LocaleStringHelper<LocaleStringResource>> ResourceHelperAsync()
        {
            return new LocaleStringHelper<LocaleStringResource>
            (
                pluginAssembly: GetType().Assembly,
                languageCultures: from lang in await _languageService.GetAllLanguagesAsync() select (lang.Id, lang.LanguageCulture),
                getResource: (resourceName, languageId) => _localizationService.GetLocaleStringResourceByNameAsync(resourceName, languageId, false),
                createResource: (languageId, resourceName, resourceValue) => new LocaleStringResource { LanguageId = languageId, ResourceName = resourceName, ResourceValue = resourceValue },
                insertResource: _localizationService.InsertLocaleStringResourceAsync,
                updateResource: (lsr, resourceValue) => { lsr.ResourceValue = resourceValue; return _localizationService.UpdateLocaleStringResourceAsync(lsr); },
                deleteResource: _localizationService.DeleteLocaleStringResourceAsync,
                areResourcesEqual: (lsr, resourceValue) => lsr.ResourceValue == resourceValue
            );
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public async Task<IList<string>> GetWidgetZonesAsync()
        {
            if (_widgetZones == null)
            {
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);

                _widgetZones = string.IsNullOrWhiteSpace(settings.WidgetZones)
                        ? []
                        : await settings.WidgetZones.Split(';').ToListAsync();
            }
            return _widgetZones;
        }
        List<string> _widgetZones;

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl() => $"{_webHelper.GetStoreLocation()}Admin/Employees/Configure";

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new EmployeeWidgetSettings
            {
                WidgetZones = "header_menu_after",
            });

            await (await ResourceHelperAsync()).CreateLocaleStringsAsync();
#if NOP_47
            await _permissionService.InstallPermissionsAsync(new EmployeePermissionProvider());
#endif

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<EmployeeWidgetSettings>();

            await (await ResourceHelperAsync()).DeleteLocaleStringsAsync();

#if NOP_48
            //delete permissions
            foreach (var name in new string[] { EmployeePermissionConfigs.MANAGE_EMPLOYEES, EmployeePermissionConfigs.MANAGE_DEPARTMENTS })
            {
                var permissionRecord = (await _permissionService.GetAllPermissionRecordsAsync())
                    .FirstOrDefault(x => x.SystemName == name);
                await _permissionService.DeletePermissionRecordAsync(permissionRecord);
            }
#endif

            // TODO: uninstall permissions
            await base.UninstallAsync();
        }

        public Type GetWidgetViewComponent(string widgetZone) => typeof(WidgetsEmployeesViewComponent);

#if NOP_47
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
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

            async Task<string> T(string format) => await _localizationService.GetResourceAsync(format) ?? format;

            foreach (var item in new List<(string caption, string controller, string action)>
            {
                (await T(EmployeeResources.ListCaption), EmployeesController.ControllerName, nameof(EmployeesController.List)),
                (await T(AdminResources.DepartmentListCaption), DepartmentsController.ControllerName, nameof(DepartmentsController.List)),
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
#endif
    }
}
