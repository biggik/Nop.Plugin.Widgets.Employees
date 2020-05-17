//using System;
//using System.Web.Routing;
//using Nop.Core.Infrastructure;
//using Nop.Core.Plugins;
//using Nop.Plugin.Widgets.Employees.Data;
//using Nop.Plugin.Widgets.Employees.Services;
//using Nop.Services.Configuration;
//using Nop.Services.Cms;
//using System.Collections.Generic;
//using Nop.Services.Localization;
//using Nop.Core.Domain.Localization;
//using System.Threading;
using System.Collections.Generic;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.Widgets.Employees.Data;
using Nop.Plugin.Widgets.Employees.Services;
using System;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Localization;
using Nop.Core;

namespace Nop.Plugin.Widgets.Employees
{
    public class EmployeesPlugin : BasePlugin, IWidgetPlugin
    {
        private static bool resourcesCreated = false;
        private readonly IEmployeesService _employeeService;
        private readonly EmployeesSettings _employeeSettings;
        private readonly EmployeesObjectContext _objectContext;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

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
            return new List<string> { "header_menu_after" };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsEmployees/Configure";
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
                DoCreateLocaleStrings(null);
            }
        }

        private void DoCreateLocaleStrings(object state)
        { 
            var resources = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>("Department.BackToList", "Back to Department list", "Tilbaka í deildalista"),
                
                new Tuple<string, string, string>("Employee.BackToList", "Back to Employee list", "Tilbaka í starfsmannalista"),
                new Tuple<string, string, string>("Employee.Department", "Department", "Deild"),
                new Tuple<string, string, string>("Employee.Employee", "Employee", "Starfsmaður"),
                new Tuple<string, string, string>("Employee.Edit", "Edit employee", "Breyta starfsmanni"),

                new Tuple<string, string, string>("Admin.Employee.Addnew", "Add new", "Skrá starfsmann"),
                new Tuple<string, string, string>("Admin.Employee.Edit", "Edit employee", "Breyta starfsmanni"),
                
                new Tuple<string, string, string>("Admin.Department.Addnew", "Add new", "Skrá deild"),
                new Tuple<string, string, string>("Admin.Department.Edit", "Edit department", "Breyta deild"),
                new Tuple<string, string, string>("Admin.Department.List", "Department list", "Deildalisti"),

                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Deleted", "Deleted", "Eytt"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Department", "Department", "Deild"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Email", "E-mail address", "Netfang"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Info", "Info", "Upplýsingar"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Interests", "Interests", "Áhugamál"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.MobileNumber", "Mobile number", "Farsími"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Name", "Name", "Nafn"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.PhoneNumber", "Phone number", "Sími"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.PictureId", "Picture", "Mynd"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Published", "Published", "Birt"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Specialties", "Specialties", "Sérhæfing"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Title", "Title", "Titill"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.WorkStarted", "Started work", "Hóf störf"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.WorkEnded", "Ended work", "Lauk störfum"),
                
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Deleted.Hint", "Is the employee deleted", "Er búið að eyða starfsmanninum"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Department.Hint", "The department the employee belongs to", "Deildin sem starfsmaðurinn tilheyrir"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Email.Hint", "The employee's e-mail address", "Netfang starfsmannsins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Info.Hint", "Any public information for the employee", "Upplýsingar um starfsmanninn"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Interests.Hint", "The interests of the employee", "Áhugamál starfsmannsins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.MobileNumber.Hint", "The employee's mobile number", "Farsími starfsmannsins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Name.Hint", "The employee's name", "Nafn starfsmannsins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.PhoneNumber.Hint", "The employee's phone number", "Sími starfsmannsins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.PictureId.Hint", "The employee's picture", "Mynd af starfsmanninum"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Published.Hint", "Should the employee be displayed", "Á að birta starfsmanninn?"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Specialties.Hint", "Any specialities of the employee", "Sérhæfing starfsmannssins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.Title.Hint", "The employee's title", "Titill starfsmannsins"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.WorkStarted.Hint", "When did the employee start work for the company", "Hvenær hóf starfsmaðurinn störf"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.WorkEnded.Hint", "If the employee doesn't work for the company anymore, when did his employment end?", "Ef starfsmaðurinn starfar ekki lengur hjá fyrirtæki, hvenær lauk hann þá störfum?"),

                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentName", "Name", "Nafn"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentPictureId", "Picture", "Mynd"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentDisplayOrder", "Display order", "Birtingarröð"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentPublished", "Published", "Birt"),

                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentName.Hint", "The department name", "Nafn deildarinnar"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentPictureId.Hint", "A picture for the department", "Mynd fyrir deildina"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentDisplayOrder.Hint", "The display order for the department", "Birtingarröð deildarinnar"),
                new Tuple<string, string, string>("Plugins.Widgets.Employees.Fields.DepartmentPublished.Hint", "Should the department be displayed?", "Á að birta deildina?")
            };

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();

            Language enUS = null;
            Language isIS = null;
            foreach (var lang in languageService.GetAllLanguages(true))
            {
                if (lang.LanguageCulture == "is-IS")
                {
                    isIS = lang;
                }
                else if (lang.LanguageCulture == "en-US")
                {
                    enUS = lang;
                }
            }

            foreach (var res in resources)
            {
                if (enUS != null)
                {
                    var lsr = localizationService.GetLocaleStringResourceByName(res.Item1, enUS.Id, false);
                    if (lsr == null)
                    {
                        localizationService.InsertLocaleStringResource(new Core.Domain.Localization.LocaleStringResource
                        {
                            Language = enUS,
                            ResourceName = res.Item1,
                            ResourceValue = res.Item2
                        });
                    }
                }

                if (isIS != null)
                {
                    var lsr = localizationService.GetLocaleStringResourceByName(res.Item1, isIS.Id, false);
                    if (lsr == null)
                    {
                        localizationService.InsertLocaleStringResource(new Core.Domain.Localization.LocaleStringResource
                        {
                            Language = isIS,
                            ResourceName = res.Item1,
                            ResourceValue = res.Item3
                        });
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
    }
}
