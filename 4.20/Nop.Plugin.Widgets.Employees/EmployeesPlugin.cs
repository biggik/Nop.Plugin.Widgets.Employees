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

namespace Nop.Plugin.Widgets.Employees
{
    public class EmployeesPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private static bool resourcesCreated = false;
        private readonly EmployeesObjectContext _objectContext;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public bool HideInWidgetList => false;

        public EmployeesPlugin(
            IWebHelper webHelper,
            EmployeesObjectContext objectContext,
            IStoreContext storeContext,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _objectContext = objectContext;
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
            _settingService.SaveSetting(new EmployeeWidgetSettings { });

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
            _settingService.DeleteSetting<EmployeeWidgetSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales
            //this.DeletePluginLocaleResource("Plugins.Widgets.Employees.Fields.Country");

            base.Uninstall();
        }

        public string GetWidgetViewComponentName(string widgetZone) => "WidgetsEmployees";

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            // TODO
        }
    }
}
