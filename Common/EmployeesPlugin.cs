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
            void Create(string resourceName, int languageId, string resourceValue)
            {
                var lsr = _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = languageId,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    _localizationService.InsertLocaleStringResource(lsr);
                }
                else if (lsr.ResourceValue != resourceValue)
                {
                    lsr.ResourceValue = resourceValue;
                    _localizationService.UpdateLocaleStringResource(lsr);
                }
            }

            ManageLocaleStrings(Create);
        }

        private void ManageLocaleStrings(Action<string, int, string> action)
        {
            foreach (var resource in PluginResources)
            {
                foreach (var resourceLanguage in resource.localeStrings)
                {
                    action.Invoke(resource.resourceName, resourceLanguage.language.Id, resourceLanguage.lsa.Value);

                    if (!string.IsNullOrWhiteSpace(resourceLanguage.lsa.Hint))
                    {
                        action.Invoke(resource.resourceName + ".Hint", resourceLanguage.language.Id, resourceLanguage.lsa.Hint);
                    }
                }
            }
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

            void Delete(string resourceName, int languageId, string resourceValue)
            {
                var lsr = _localizationService.GetLocaleStringResourceByName(resourceName, languageId, false);
                if (lsr != null)
                {
                    _localizationService.DeleteLocaleStringResource(lsr);
                }
            }

            ManageLocaleStrings(Delete);
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

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
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

        private IEnumerable<(string resourceName, IEnumerable<(LocaleStringAttribute lsa, Language language)> localeStrings)> PluginResources
        {
            get
            {
                var availableLanguages = (from lang in _languageService.GetAllLanguages()
                                          select lang
                                         ).ToDictionary(x => x.LanguageCulture, y => y);

                IEnumerable<(LocaleStringAttribute lsa, Language language)> AsLSA(object[] values)
                {
                    foreach (var v in values)
                    {
                        var lsa = v as LocaleStringAttribute;
                        if (availableLanguages.ContainsKey(lsa.Culture))
                        {
                            yield return (lsa, availableLanguages[lsa.Culture]);
                        }
                    }
                }

                return from type in GetType().Assembly.GetTypes()
                       where type.CustomAttributes.Any(x => x.AttributeType == typeof(LocaleStringProviderAttribute))
                       from field in type.GetFields()
                       select (resourceName: field.GetValue(null).ToString(),
                               localeStrings: AsLSA(field.GetCustomAttributes(typeof(LocaleStringAttribute), false)));
            }
        }

    }
}
