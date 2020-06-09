using System;

namespace Nop.Plugin.Widgets.Employees.Resources
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LocaleStringProviderAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LocaleStringAttribute : Attribute
    {
        public const string EN = "en-US"; 
        public const string IS = "is-IS";
        public LocaleStringAttribute(string culture, string value, string hint = null)
        {
            Culture = culture;
            Value = value;
            Hint = hint;
        }
        public string Culture { get; }
        public string Value { get; }
        public string Hint { get; }
    }

    [LocaleStringProvider]
    public static class EmployeeResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Details")]
        [LocaleString(LocaleStringAttribute.IS, "Nánar")]
        public const string Details = "Status.EmployeeWidget.Employee.Details";

        [LocaleString(LocaleStringAttribute.EN, "Department", "The department the employee belongs to")]
        [LocaleString(LocaleStringAttribute.IS, "Deild", "Deildin sem starfsmaðurinn tilheyrir")]
        public const string Department = "Status.EmployeeWidget.Employee.Department";

        [LocaleString(LocaleStringAttribute.EN, "Name", "The employee's name")]
        [LocaleString(LocaleStringAttribute.IS, "Nafn", "Nafn starfsmannsins")]
        public const string Name = "Status.EmployeeWidget.Employee.Name";

        [LocaleString(LocaleStringAttribute.EN, "Title", "The employee's title")]
        [LocaleString(LocaleStringAttribute.IS, "Titill", "Titill starfsmannsins")]
        public const string Title = "Status.EmployeeWidget.Employee.Title";

        [LocaleString(LocaleStringAttribute.EN, "E-mail address", "The employee's e-mail address")]
        [LocaleString(LocaleStringAttribute.IS, "Netfang", "Netfang starfsmannsins")]
        public const string Email = "Status.EmployeeWidget.Employee.Email";

        [LocaleString(LocaleStringAttribute.EN, "Picture", "The employee's picture")]
        [LocaleString(LocaleStringAttribute.IS, "Mynd", "Mynd af starfsmanninum")]
        public const string PictureId = "Status.EmployeeWidget.Employee.PictureId";

        [LocaleString(LocaleStringAttribute.EN, "Info", "Any public information for the employee")]
        [LocaleString(LocaleStringAttribute.IS, "Upplýsingar", "Upplýsingar um starfsmanninn")]
        public const string Info = "Status.EmployeeWidget.Employee.Info";

        [LocaleString(LocaleStringAttribute.EN, "Specialties", "Any specialities of the employee")]
        [LocaleString(LocaleStringAttribute.IS, "Sérhæfing", "Sérhæfing starfsmannssins")]
        public const string Specialties = "Status.EmployeeWidget.Employee.Specialties";

        [LocaleString(LocaleStringAttribute.EN, "Interests", "The interests of the employee")]
        [LocaleString(LocaleStringAttribute.IS, "Áhugamál", "Áhugamál starfsmannsins")]
        public const string Interests = "Status.EmployeeWidget.Employee.Interests";

        [LocaleString(LocaleStringAttribute.EN, "Phone number", "The employee's phone number")]
        [LocaleString(LocaleStringAttribute.IS, "Sími", "Sími starfsmannsins")]
        public const string PhoneNumber = "Status.EmployeeWidget.Employee.PhoneNumber";

        [LocaleString(LocaleStringAttribute.EN, "Mobile number", "The employee's mobile number")]
        [LocaleString(LocaleStringAttribute.IS, "Farsími", "Farsími starfsmannsins")]
        public const string MobileNumber = "Status.EmployeeWidget.Employee.MobileNumber";

        [LocaleString(LocaleStringAttribute.EN, "Started work", "When did the employee start work for the company")]
        [LocaleString(LocaleStringAttribute.IS, "Hóf störf", "Hvenær hóf starfsmaðurinn störf")]
        public const string WorkStarted = "Status.EmployeeWidget.Employee.WorkStarted";

        [LocaleString(LocaleStringAttribute.EN, "Ended work", "If the employee doesn't work for the company anymore, when did his employment end?")]
        [LocaleString(LocaleStringAttribute.IS, "Lauk störfum", "Ef starfsmaðurinn starfar ekki lengur hjá fyrirtæki, hvenær lauk hann þá störfum?")]
        public const string WorkEnded = "Status.EmployeeWidget.Employee.WorkEnded";

        [LocaleString(LocaleStringAttribute.EN, "Employees")]
        [LocaleString(LocaleStringAttribute.IS, "Starfsmenn")]
        public const string ListCaption = "Status.EmployeeWidget.Employee.Caption.Employees";
    }

    [LocaleStringProvider]
    public static class EmployeeActionResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Employees")]
        [LocaleString(LocaleStringAttribute.IS, "Starfsmenn")]
        public const string EmployeeListButton = "Status.EmployeeWidget.Action.Employees.List";

        [LocaleString(LocaleStringAttribute.EN, "Back to Employee list")]
        [LocaleString(LocaleStringAttribute.IS, "Tilbaka í starfsmannalista")]
        public const string BackToList = "Status.EmployeeWidget.Action.Employee.BackToList";

        [LocaleString(LocaleStringAttribute.EN, "View picture")]
        [LocaleString(LocaleStringAttribute.IS, "Skoða mynd")]
        public const string ViewPicture = "Status.EmployeeWidget.Action.Employee.ViewPicture";
    }

    [LocaleStringProvider]
    public static class AdminResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Employee list")]
        [LocaleString(LocaleStringAttribute.IS, "Starfsmannalisti")]
        public const string EmployeeListCaption = "Status.EmployeeWidget.Admin.Employee.Caption.Employees";

        [LocaleString(LocaleStringAttribute.EN, "Add a new employee")]
        [LocaleString(LocaleStringAttribute.IS, "Skrá nýjan starfsmann")]
        public const string AddEmployeeCaption = "Status.EmployeeWidget.Admin.Employee.Caption.Add";

        [LocaleString(LocaleStringAttribute.EN, "Add new")]
        [LocaleString(LocaleStringAttribute.IS, "Skrá starfsmann")]
        public const string AddEmployeeButton = "Status.EmployeeWidget.Admin.Employee.Button.Add";

        [LocaleString(LocaleStringAttribute.EN, "See published")]
        [LocaleString(LocaleStringAttribute.IS, "Sjá virka")]
        public const string SeePublished = "Status.EmployeeWidget.Admin.Employee.Button.SeePublished";

        [LocaleString(LocaleStringAttribute.EN, "See all")]
        [LocaleString(LocaleStringAttribute.IS, "Sjá alla")]
        public const string SeeAllButton = "Status.EmployeeWidget.Admin.Employee.Button.SeeAll";

        [LocaleString(LocaleStringAttribute.EN, "Edit employee")]
        [LocaleString(LocaleStringAttribute.IS, "Breyta starfsmanni")]
        public const string EditEmployeeCaption = "Status.EmployeeWidget.Admin.Employee.Caption.Edit";

        [LocaleString(LocaleStringAttribute.EN, "Add new")]
        [LocaleString(LocaleStringAttribute.IS, "Skrá deild")]
        public const string AddDepartmentCaption = "Status.EmployeeWidget.Admin.Department.Caption.Add";

        [LocaleString(LocaleStringAttribute.EN, "Add new")]
        [LocaleString(LocaleStringAttribute.IS, "Skrá deild")]
        public const string AddDepartmentButton = "Status.EmployeeWidget.Admin.Department.Button.Add";

        [LocaleString(LocaleStringAttribute.EN, "Edit department")]
        [LocaleString(LocaleStringAttribute.IS, "Breyta deild")]
        public const string EditDepartmentCaption = "Status.EmployeeWidget.Admin.Department.Caption.Edit";

        [LocaleString(LocaleStringAttribute.EN, "Employee department list")]
        [LocaleString(LocaleStringAttribute.IS, "Deildalisti starfsmanna")]
        public const string DepartmentListCaption = "Status.EmployeeWidget.Admin.Department.Caption.Departments";
    }

    [LocaleStringProvider]
    public static class DepartmentResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Name", "The department name")]
        [LocaleString(LocaleStringAttribute.IS, "Nafn", "Nafn deildarinnar")]
        public const string Name = "Status.EmployeeWidget.Department.Name";

        [LocaleString(LocaleStringAttribute.EN, "Picture", "A picture for the department")]
        [LocaleString(LocaleStringAttribute.IS, "Mynd", "Mynd fyrir deildina")]
        public const string PictureId = "Status.EmployeeWidget.Department.PictureId";
    }

    [LocaleStringProvider]
    public static class DepartmentActionResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Back to Department list")]
        [LocaleString(LocaleStringAttribute.IS, "Tilbaka í deildalista")]
        public const string BackToList = "Status.EmployeeWidget.Action.Department.BackToList";

        [LocaleString(LocaleStringAttribute.EN, "Departments")]
        [LocaleString(LocaleStringAttribute.IS, "Deildir")]
        public const string DepartmentListButton = "Status.EmployeeWidget.Action.Department.List";
    }

    [LocaleStringProvider]
    public static class ConfigurationResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Widget zones", "In which zones should the widget be displayed")]
        [LocaleString(LocaleStringAttribute.IS, "Birta í", "Hvar á síðunni á að birta íhlutinn")]
        public const string WidgetZones = "Status.EmployeeWidget.Configuration.WidgetZones";
    }

    [LocaleStringProvider]
    public static class GenericResources
    {
        [LocaleString(LocaleStringAttribute.EN, "Display order", "The display order for the record")]
        [LocaleString(LocaleStringAttribute.IS, "Birtingarröð", "Birtingarröð færslunnar")]
        public const string DisplayOrder = "Status.EmployeeWidget.Generic.DisplayOrder";

        [LocaleString(LocaleStringAttribute.EN, "Published", "Should the record be displayed?")]
        [LocaleString(LocaleStringAttribute.IS, "Birt", "Á að birta færsluna?")]
        public const string Published = "Status.EmployeeWidget.Generic.Published";
    }
}
