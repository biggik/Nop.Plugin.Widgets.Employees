using System.Linq;
using System.Collections.Generic;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.Widgets.Employees.Data;
using Nop.Plugin.Widgets.Employees.Services;
using System;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Localization;
using Nop.Core;
using Nop.Services.Plugins;
using Nop.Plugin.Widgets.Employees.Resources;
using Nop.Web.Framework.Menu;
using Nop.Plugin.Widgets.Employees.Controllers;

namespace Nop.Plugin.Widgets.Employees
{
    public class EmployeesPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private static bool resourcesCreated = false;
        private readonly IEmployeesService _employeeService;
        private readonly EmployeesSettings _employeeSettings;
        private readonly EmployeesObjectContext _objectContext;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public bool HideInWidgetList => false;

        public EmployeesPlugin(
            IWebHelper webHelper,
            IEmployeesService employeeService,
            EmployeesSettings employeeSettings,
            EmployeesObjectContext objectContext,
            ISettingService settingService)
        {
            this._webHelper = webHelper;
            this._employeeService = employeeService;
            this._employeeSettings = employeeSettings;
            this._objectContext = objectContext;
            this._settingService = settingService;
#if DEBUG
            CreateLocaleStrings();
#endif
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new string[] { };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Employees/Configure";
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        //public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        //{
        //    actionName = "Configure";
        //    controllerName = "Employees";
        //    routeValues = new RouteValueDictionary
        //    {
        //        { "Namespaces", "Nop.Plugin.Widgets.Employees.Controllers" }, 
        //        { "area", "admin" }
        //    };
        //}

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        //public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        //{
        //    actionName = "PublicInfo";
        //    controllerName = "Employees";
        //    routeValues = new RouteValueDictionary
        //    {
        //        {"Namespaces", "Nop.Plugin.Widgets.Employees.Controllers"},
        //        {"area", null},
        //        {"widgetZone", widgetZone}
        //    };
        //}

        internal void CreateLocaleStrings()
        {
            if (!resourcesCreated)
            {
                resourcesCreated = true;
                DoCreateLocaleStrings();
            }
        }

        private void DoCreateLocaleStrings()
        { 
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();

            var availableLanguages = (from lang in languageService.GetAllLanguages()
                                      select lang).ToDictionary(x => x.LanguageCulture, y => y);

            IEnumerable<(LocaleStringAttribute lsa, Language language)> AsLSA(object[] values)
            {
                foreach (var v in values)
                {
                    var lsa = (LocaleStringAttribute)v;
                    if (availableLanguages.ContainsKey(lsa.Culture))
                    {
                        yield return (lsa, availableLanguages[lsa.Culture]);
                    }
                }
            }
            var resources = from type in GetType().Assembly.GetTypes()
                            where type.CustomAttributes.Any(x => x.AttributeType == typeof(LocaleStringProviderAttribute))
                            from field in type.GetFields()
                            select (resourceName: field.GetValue(null).ToString(), 
                                    localeStrings: AsLSA(field.GetCustomAttributes(typeof(LocaleStringAttribute), false)));

            foreach (var resource in resources)
            {
                foreach (var resourceLanguage in resource.localeStrings)
                {
                    void Create(string resourceName, string resourceValue)
                    {
                        var lsr = localizationService.GetLocaleStringResourceByName(resourceName, resourceLanguage.language.Id, false);
                        if (lsr == null)
                        {
                            lsr = new LocaleStringResource
                            {
                                LanguageId = resourceLanguage.language.Id,
                                ResourceName = resourceName,
                                ResourceValue = resourceValue
                            };
                            localizationService.InsertLocaleStringResource(lsr);
                        }
                        else if (lsr.ResourceValue != resourceValue)
                        {
                            lsr.ResourceValue = resourceValue;
                            localizationService.UpdateLocaleStringResource(lsr);
                        }
                    }
                    Create(resource.resourceName, resourceLanguage.lsa.Value);

                    if (!string.IsNullOrWhiteSpace(resourceLanguage.lsa.Hint))
                    {
                        Create(resource.resourceName + ".Hint", resourceLanguage.lsa.Hint);
                    }
                }
            }
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new EmployeesSettings
            {
                LimitMethodsToCreated = false,
            };
            _settingService.SaveSetting(settings);

            //database objects
            _objectContext.Install();

            CreateLocaleStrings();
            
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<EmployeesSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales
            //this.DeletePluginLocaleResource("Plugins.Widgets.Employees.Fields.Country");
            
            base.Uninstall();
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsEmployees";
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            // TODO
        }
    }
}
