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
            CreateMap<EmployeeModel, Employee>()
                .ForMember(dest => dest.Deleted, mo => mo.Ignore());
            CreateMap<Employee, EmployeeModel>()
                .ForMember(dest => dest.AvailableDepartments, mo => mo.Ignore())
                .ForMember(dest => dest.PhotoUrl, mo => mo.Ignore())
                .ForMember(dest => dest.DepartmentName, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            
            //countries
            CreateMap<DepartmentModel, Department>();
            CreateMap<Department, DepartmentModel>()
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