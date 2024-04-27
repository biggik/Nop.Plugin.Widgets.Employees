using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.Employees.Constants;

internal static class Areas
{
#if NOP_47
    internal const string Admin = AreaNames.ADMIN;
#else
    internal const string Admin = AreaNames.Admin;
#endif
}
