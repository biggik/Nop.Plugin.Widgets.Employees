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
        private readonly IStoreContext _storeContext;

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
            _storeContext = storeContext;
#if NOP_PRE_4_3
            _objectContext = objectContext;
#endif
            _settingService = settingService;

#if DEBUG
            DebugInitialize();
#endif
        }

#if DEBUG
        private static bool _debugInitialized = false;

        private void DebugInitialize()
        {
            if (_debugInitialized)
                return;

            _debugInitialized = true;
#if NOP_ASYNC
            ResourceHelper().CreateLocaleStringsAsync();
            _permissionService.InstallPermissionsAsync(new EmployeePermissionProvider());
#else
            ResourceHelper().CreateLocaleStrings();
            _permissionService.InstallPermissions(new EmployeePermissionProvider());
#endif
        }
#endif

        private LocaleStringHelper<LocaleStringResource> ResourceHelper()
        {
            return new LocaleStringHelper<LocaleStringResource>
            (
#if NOP_ASYNC
                pluginAssembly: GetType().Assembly,
                languageCultures: from lang in _languageService.GetAllLanguagesAsync().Result select (lang.Id, lang.LanguageCulture),
                getResource: (resourceName, languageId) => _localizationService.GetLocaleStringResourceByNameAsync(resourceName, languageId, false),
                createResource: (languageId, resourceName, resourceValue) => new LocaleStringResource { LanguageId = languageId, ResourceName = resourceName, ResourceValue = resourceValue },
                insertResource: (lsr) => _localizationService.InsertLocaleStringResourceAsync(lsr),
                updateResource: (lsr, resourceValue) => { lsr.ResourceValue = resourceValue; return _localizationService.UpdateLocaleStringResourceAsync(lsr); },
                deleteResource: (lsr) => _localizationService.DeleteLocaleStringResourceAsync(lsr),
                areResourcesEqual: (lsr, resourceValue) => lsr.ResourceValue == resourceValue
#else
                pluginAssembly: GetType().Assembly,
                languageCultures: from lang in _languageService.GetAllLanguages() select (lang.Id, lang.LanguageCulture),
                getResource: (resourceName, languageId) => _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false),
                createResource: (languageId, resourceName, resourceValue) => new LocaleStringResource { LanguageId = languageId, ResourceName = resourceName, ResourceValue = resourceValue },
                insertResource: (lsr) => _localizationService.InsertLocaleStringResource(lsr),
                updateResource: (lsr, resourceValue) => { lsr.ResourceValue = resourceValue; _localizationService.UpdateLocaleStringResource(lsr); },
                deleteResource: (lsr) => _localizationService.DeleteLocaleStringResource(lsr),
                areResourcesEqual: (lsr, resourceValue) => lsr.ResourceValue == resourceValue
#endif
            );
        }

                /// <summary>
                /// Gets widget zones where this widget should be rendered
                /// </summary>
                /// <returns>Widget zones</returns>
#if NOP_ASYNC
            public async Task<IList<string>> GetWidgetZonesAsync()
        {
            if (_widgetZones == null)
            {
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var settings = await _settingService.LoadSettingAsync<EmployeeWidgetSettings>(storeScope);

                _widgetZones = string.IsNullOrWhiteSpace(settings.WidgetZones)
                        ? new List<string>()
                        : settings.WidgetZones.Split(';').ToList();
            }
            return _widgetZones;
        }
#else
        public IList<string> GetWidgetZones()
        {
            if (_widgetZones == null)
            {
                var storeScope = _storeContext.ActiveStoreScopeConfiguration;
                var settings = _settingService.LoadSetting<EmployeeWidgetSettings>(storeScope);

                _widgetZones = string.IsNullOrWhiteSpace(settings.WidgetZones)
                        ? new List<string>()
                        : settings.WidgetZones.Split(';').ToList();
            }
            return _widgetZones;
        }
#endif
        List<string> _widgetZones;



        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl() => $"{_webHelper.GetStoreLocation()}Admin/Employees/Configure";

        /// <summary>
        /// Install plugin
        /// </summary>
#if NOP_ASYNC
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new EmployeeWidgetSettings
            {
                WidgetZones = "header_menu_after",
            });

#if NOP_ASYNC
            await ResourceHelper().CreateLocaleStringsAsync();
#else
            ResourceHelper().CreateLocaleStrings();
#endif
            await _permissionService.InstallPermissionsAsync(new EmployeePermissionProvider());

            await base.InstallAsync();
        }
#else
        public override void Install()
        {
            _settingService.SaveSetting(new EmployeeWidgetSettings
            {
                WidgetZones = "header_menu_after",
            });

#if NOP_PRE_4_3
            _objectContext.Install();
#endif

#if NOP_ASYNC
            await ResourceHelper().CreateLocaleStringsAsync();
#else
            ResourceHelper().CreateLocaleStrings();
#endif
            _permissionService.InstallPermissions(new EmployeePermissionProvider());

            base.Install();
        }
#endif

        /// <summary>
        /// Uninstall plugin
        /// </summary>
#if NOP_ASYNC
        public override async Task UninstallAsync()
#else
        public override void Uninstall()
#endif
        {
            //settings
#if NOP_ASYNC
            await _settingService.DeleteSettingAsync<EmployeeWidgetSettings>();
#else
            _settingService.DeleteSetting<EmployeeWidgetSettings>();
#endif

#if NOP_PRE_4_3
            _objectContext.Uninstall();
#endif

#if NOP_ASYNC
            await ResourceHelper().DeleteLocaleStringsAsync();
#else
            ResourceHelper().DeleteLocaleStrings();
#endif

#if NOP_ASYNC
            // TODO: uninstall permissions
            await base.UninstallAsync();
#else
            _permissionService.UninstallPermissions(new EmployeePermissionProvider());

            base.Uninstall();
#endif
        }
#if NOP_46
        public Type GetWidgetViewComponent(string widgetZone) => typeof(WidgetsEmployeesViewComponent);
#else

        public string GetWidgetViewComponentName(string widgetZone) => "WidgetsEmployees";
#endif

#if NOP_ASYNC
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
#else
        public void ManageSiteMap(SiteMapNode rootNode)
#endif
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

#if NOP_ASYNC
            async Task<string> T(string format) => await _localizationService.GetResourceAsync(format) ?? format;
#else
            string T(string format) => _localizationService.GetResource(format) ?? format;
#endif

            foreach (var item in new List<(string caption, string controller, string action)>
            {
#if NOP_ASYNC
                (await T(EmployeeResources.ListCaption), EmployeesController.ControllerName, nameof(EmployeesController.List)),
                (await T(AdminResources.DepartmentListCaption), DepartmentsController.ControllerName, nameof(DepartmentsController.List)),
#else
                (T(EmployeeResources.ListCaption), EmployeesController.ControllerName, nameof(EmployeesController.List)),
                (T(AdminResources.DepartmentListCaption), DepartmentsController.ControllerName, nameof(DepartmentsController.List)),
#endif
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
