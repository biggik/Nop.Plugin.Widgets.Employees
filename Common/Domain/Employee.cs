using System;
using Nop.Core;

namespace Nop.Plugin.Widgets.Employees.Domain
{
    public partial class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public int PictureId { get; set; }
        public string Info { get; set; }
        public string Specialties { get; set; }

        public string Interests { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public DateTime WorkStarted { get; set; }
        public DateTime? WorkEnded { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
    }
}
