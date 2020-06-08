using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Models
{
    public static class ModelExtender
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        public static EmployeeModel ToModel(this Employee entity)
        {
            return entity.MapTo<Employee, EmployeeModel>();
        }

        public static Employee ToEntity(this EmployeeModel model)
        {
            return model.MapTo<EmployeeModel, Employee>();
        }

        public static Employee ToEntity(this EmployeeModel model, Employee destination)
        {
            return model.MapTo(destination);
        }

        public static DepartmentModel ToModel(this Department entity)
        {
            return entity.MapTo<Department, DepartmentModel>();
        }

        public static Department ToEntity(this DepartmentModel model)
        {
            return model.MapTo<DepartmentModel, Department>();
        }

        public static Department ToEntity(this DepartmentModel model, Department destination)
        {
            return model.MapTo(destination);
        }
    }
}
