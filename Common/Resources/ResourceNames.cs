using nopLocalizationHelper;

namespace Nop.Plugin.Widgets.Employees.Resources
{
    internal static class Cultures
    {
        public const string EN = "en-US";
        public const string IS = "is-IS";
    }

    [LocaleStringProvider]
    public static class EmployeeResources
    {
        [LocaleString(Cultures.EN, "Details")]
        [LocaleString(Cultures.IS, "Nánar")]
        public const string Details = "Status.EmployeeWidget.Employee.Details";

        [LocaleString(Cultures.EN, "Department", "The department the employee belongs to")]
        [LocaleString(Cultures.IS, "Deild", "Deildin sem starfsmaðurinn tilheyrir")]
        public const string Department = "Status.EmployeeWidget.Employee.Department";

        [LocaleString(Cultures.EN, "Name", "The employee's name")]
        [LocaleString(Cultures.IS, "Nafn", "Nafn starfsmannsins")]
        public const string Name = "Status.EmployeeWidget.Employee.Name";

        [LocaleString(Cultures.EN, "Title", "The employee's title")]
        [LocaleString(Cultures.IS, "Titill", "Titill starfsmannsins")]
        public const string Title = "Status.EmployeeWidget.Employee.Title";

        [LocaleString(Cultures.EN, "E-mail address", "The employee's e-mail address")]
        [LocaleString(Cultures.IS, "Netfang", "Netfang starfsmannsins")]
        public const string Email = "Status.EmployeeWidget.Employee.Email";

        [LocaleString(Cultures.EN, "Picture", "The employee's picture")]
        [LocaleString(Cultures.IS, "Mynd", "Mynd af starfsmanninum")]
        public const string PictureId = "Status.EmployeeWidget.Employee.PictureId";

        [LocaleString(Cultures.EN, "Info", "Any public information for the employee")]
        [LocaleString(Cultures.IS, "Upplýsingar", "Upplýsingar um starfsmanninn")]
        public const string Info = "Status.EmployeeWidget.Employee.Info";

        [LocaleString(Cultures.EN, "Specialties", "Any specialities of the employee")]
        [LocaleString(Cultures.IS, "Sérhæfing", "Sérhæfing starfsmannssins")]
        public const string Specialties = "Status.EmployeeWidget.Employee.Specialties";

        [LocaleString(Cultures.EN, "Interests", "The interests of the employee")]
        [LocaleString(Cultures.IS, "Áhugamál", "Áhugamál starfsmannsins")]
        public const string Interests = "Status.EmployeeWidget.Employee.Interests";

        [LocaleString(Cultures.EN, "Phone number", "The employee's phone number")]
        [LocaleString(Cultures.IS, "Sími", "Sími starfsmannsins")]
        public const string PhoneNumber = "Status.EmployeeWidget.Employee.PhoneNumber";

        [LocaleString(Cultures.EN, "Mobile number", "The employee's mobile number")]
        [LocaleString(Cultures.IS, "Farsími", "Farsími starfsmannsins")]
        public const string MobileNumber = "Status.EmployeeWidget.Employee.MobileNumber";

        [LocaleString(Cultures.EN, "Started work", "When did the employee start work for the company")]
        [LocaleString(Cultures.IS, "Hóf störf", "Hvenær hóf starfsmaðurinn störf")]
        public const string WorkStarted = "Status.EmployeeWidget.Employee.WorkStarted";

        [LocaleString(Cultures.EN, "Ended work", "If the employee doesn't work for the company anymore, when did his employment end?")]
        [LocaleString(Cultures.IS, "Lauk störfum", "Ef starfsmaðurinn starfar ekki lengur hjá fyrirtæki, hvenær lauk hann þá störfum?")]
        public const string WorkEnded = "Status.EmployeeWidget.Employee.WorkEnded";

        [LocaleString(Cultures.EN, "Employees")]
        [LocaleString(Cultures.IS, "Starfsmenn")]
        public const string ListCaption = "Status.EmployeeWidget.Employee.Caption.Employees";
    }

    [LocaleStringProvider]
    public static class EmployeeActionResources
    {
        [LocaleString(Cultures.EN, "Employees")]
        [LocaleString(Cultures.IS, "Starfsmenn")]
        public const string EmployeeListButton = "Status.EmployeeWidget.Action.Employees.List";

        [LocaleString(Cultures.EN, "Back to Employee list")]
        [LocaleString(Cultures.IS, "Tilbaka í starfsmannalista")]
        public const string BackToList = "Status.EmployeeWidget.Action.Employee.BackToList";

        [LocaleString(Cultures.EN, "View picture")]
        [LocaleString(Cultures.IS, "Skoða mynd")]
        public const string ViewPicture = "Status.EmployeeWidget.Action.Employee.ViewPicture";
    }

    [LocaleStringProvider]
    public static class AdminResources
    {
        [LocaleString(Cultures.EN, "Employee list")]
        [LocaleString(Cultures.IS, "Starfsmannalisti")]
        public const string EmployeeListCaption = "Status.EmployeeWidget.Admin.Employee.Caption.Employees";

        [LocaleString(Cultures.EN, "Add a new employee")]
        [LocaleString(Cultures.IS, "Skrá nýjan starfsmann")]
        public const string AddEmployeeCaption = "Status.EmployeeWidget.Admin.Employee.Caption.Add";

        [LocaleString(Cultures.EN, "Add new")]
        [LocaleString(Cultures.IS, "Skrá starfsmann")]
        public const string AddEmployeeButton = "Status.EmployeeWidget.Admin.Employee.Button.Add";

        [LocaleString(Cultures.EN, "See published")]
        [LocaleString(Cultures.IS, "Sjá virka")]
        public const string SeePublished = "Status.EmployeeWidget.Admin.Employee.Button.SeePublished";

        [LocaleString(Cultures.EN, "See all")]
        [LocaleString(Cultures.IS, "Sjá alla")]
        public const string SeeAllButton = "Status.EmployeeWidget.Admin.Employee.Button.SeeAll";

        [LocaleString(Cultures.EN, "Edit employee")]
        [LocaleString(Cultures.IS, "Breyta starfsmanni")]
        public const string EditEmployeeCaption = "Status.EmployeeWidget.Admin.Employee.Caption.Edit";

        [LocaleString(Cultures.EN, "Add new")]
        [LocaleString(Cultures.IS, "Skrá deild")]
        public const string AddDepartmentCaption = "Status.EmployeeWidget.Admin.Department.Caption.Add";

        [LocaleString(Cultures.EN, "Add new")]
        [LocaleString(Cultures.IS, "Skrá deild")]
        public const string AddDepartmentButton = "Status.EmployeeWidget.Admin.Department.Button.Add";

        [LocaleString(Cultures.EN, "Edit department")]
        [LocaleString(Cultures.IS, "Breyta deild")]
        public const string EditDepartmentCaption = "Status.EmployeeWidget.Admin.Department.Caption.Edit";

        [LocaleString(Cultures.EN, "Employee department list")]
        [LocaleString(Cultures.IS, "Deildalisti starfsmanna")]
        public const string DepartmentListCaption = "Status.EmployeeWidget.Admin.Department.Caption.Departments";
    }

    [LocaleStringProvider]
    public static class DepartmentResources
    {
        [LocaleString(Cultures.EN, "Name", "The department name")]
        [LocaleString(Cultures.IS, "Nafn", "Nafn deildarinnar")]
        public const string Name = "Status.EmployeeWidget.Department.Name";

        [LocaleString(Cultures.EN, "Picture", "A picture for the department")]
        [LocaleString(Cultures.IS, "Mynd", "Mynd fyrir deildina")]
        public const string PictureId = "Status.EmployeeWidget.Department.PictureId";
    }

    [LocaleStringProvider]
    public static class DepartmentActionResources
    {
        [LocaleString(Cultures.EN, "Back to Department list")]
        [LocaleString(Cultures.IS, "Tilbaka í deildalista")]
        public const string BackToList = "Status.EmployeeWidget.Action.Department.BackToList";

        [LocaleString(Cultures.EN, "Departments")]
        [LocaleString(Cultures.IS, "Deildir")]
        public const string DepartmentListButton = "Status.EmployeeWidget.Action.Department.List";
    }

    [LocaleStringProvider]
    public static class ConfigurationResources
    {
        [LocaleString(Cultures.EN, "Widget zones", "In which zones should the widget be displayed")]
        [LocaleString(Cultures.IS, "Birta í", "Hvar á síðunni á að birta íhlutinn")]
        public const string WidgetZones = "Status.EmployeeWidget.Configuration.WidgetZones";
    }

    [LocaleStringProvider]
    public static class GenericResources
    {
        [LocaleString(Cultures.EN, "Display order", "The display order for the record")]
        [LocaleString(Cultures.IS, "Birtingarröð", "Birtingarröð færslunnar")]
        public const string DisplayOrder = "Status.EmployeeWidget.Generic.DisplayOrder";

        [LocaleString(Cultures.EN, "Published", "Should the record be displayed?")]
        [LocaleString(Cultures.IS, "Birt", "Á að birta færsluna?")]
        public const string Published = "Status.EmployeeWidget.Generic.Published";
    }
}
