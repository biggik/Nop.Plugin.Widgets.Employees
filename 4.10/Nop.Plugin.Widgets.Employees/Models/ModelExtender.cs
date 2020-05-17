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

        public static EmployeeModel ToModel(this EmployeesRecord entity)
        {
            return entity.MapTo<EmployeesRecord, EmployeeModel>();
        }

        public static EmployeesRecord ToEntity(this EmployeeModel model)
        {
            return model.MapTo<EmployeeModel, EmployeesRecord>();
        }

        public static EmployeesRecord ToEntity(this EmployeeModel model, EmployeesRecord destination)
        {
            return model.MapTo(destination);
        }

        public static DepartmentModel ToModel(this DepartmentRecord entity)
        {
            return entity.MapTo<DepartmentRecord, DepartmentModel>();
        }

        public static DepartmentRecord ToEntity(this DepartmentModel model)
        {
            return model.MapTo<DepartmentModel, DepartmentRecord>();
        }

        public static DepartmentRecord ToEntity(this DepartmentModel model, DepartmentRecord destination)
        {
            return model.MapTo(destination);
        }
    }
}
