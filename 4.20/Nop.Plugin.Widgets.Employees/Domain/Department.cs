using Nop.Core;

namespace Nop.Plugin.Widgets.Employees.Domain
{
    public partial class Department : BaseEntity
    {
        public string  Name { get; set; }

        public int PictureId { get; set; }
        
        public int DisplayOrder { get; set; }

        public bool Published { get; set; }
    }
}