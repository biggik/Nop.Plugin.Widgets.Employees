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
        public EmployeesMapperConfiguration()
        {
            CreateMap<EmployeeModel, Employee>();
            CreateMap<Employee, EmployeeModel>()
                .ForMember(em => em.Id, mo => mo.MapFrom(e => e.Id))
                .ForMember(em => em.DepartmentId, mo => mo.MapFrom(e => e.DepartmentId))
                .ForMember(em => em.Name, mo => mo.MapFrom(e => e.Name))
                .ForMember(em => em.Title, mo => mo.MapFrom(e => e.Title))
                .ForMember(em => em.Email, mo => mo.MapFrom(e => e.Email))
                .ForMember(em => em.PictureId, mo => mo.MapFrom(e => e.PictureId))
                .ForMember(em => em.Info, mo => mo.MapFrom(e => e.Info))
                .ForMember(em => em.Specialties, mo => mo.MapFrom(e => e.Specialties))
                .ForMember(em => em.Interests, mo => mo.MapFrom(e => e.Interests))
                .ForMember(em => em.PhoneNumber, mo => mo.MapFrom(e => e.PhoneNumber))
                .ForMember(em => em.MobileNumber, mo => mo.MapFrom(e => e.MobileNumber))
                .ForMember(em => em.WorkStarted, mo => mo.MapFrom(e => e.WorkStarted))
                .ForMember(em => em.WorkEnded, mo => mo.MapFrom(e => e.WorkEnded))
                .ForMember(em => em.Published, mo => mo.MapFrom(e => e.Published))
                .ForMember(em => em.DisplayOrder, mo => mo.MapFrom(e => e.DisplayOrder))
                ;

            CreateMap<DepartmentModel, Department>();
            CreateMap<Department, DepartmentModel>()
               .ForMember(dest => dest.PictureUrl, mo => mo.Ignore())
               .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 0;
    }
}