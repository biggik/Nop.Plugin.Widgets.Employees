using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Models
{
    /// <summary>
    /// AutoMapper configuration for Employees models
    /// </summary>
    public class EmployeesMapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor
        public EmployeesMapperConfiguration()
        {
            CreateMap<EmployeeModel, EmployeesRecord>()
                .ForMember(dest => dest.Deleted, mo => mo.Ignore());
            CreateMap<EmployeesRecord, EmployeeModel>()
                .ForMember(dest => dest.AvailableDepartments, mo => mo.Ignore())
                .ForMember(dest => dest.PhotoUrl, mo => mo.Ignore())
                .ForMember(dest => dest.DepartmentName, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            
            //countries
            CreateMap<DepartmentModel, DepartmentRecord>();
            CreateMap<DepartmentRecord, DepartmentModel>()
               .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            
        }
        #endregion

        #region Properties

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;

        #endregion
        
    }
}