using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.Employees.Models
{
#if NOP_ASYNC
    public record
#else
    public class 
#endif
        DepartmentSearchModel : BaseSearchModel
    {
    }
}
