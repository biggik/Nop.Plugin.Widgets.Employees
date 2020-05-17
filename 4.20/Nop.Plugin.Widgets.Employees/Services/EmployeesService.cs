using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial class EmployeesService : IEmployeesService
    {
        #region Constants
        private const string EMPLOYEES_ALL_KEY = "Nop.employee.all-{0}-{1}";
        private const string DEPARTMENTS_ALL_KEY = "Nop.department.all-{0}";
        private const string EMPLOYEES_PATTERN_KEY = "Nop.employee.";
        #endregion

        #region Fields
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRespository;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public EmployeesService(
            ICacheManager cacheManager,
            IRepository<Employee> employeeRepository, 
            IRepository<Department> departmentRespository)
        {
            _cacheManager = cacheManager;
            _employeeRepository = employeeRepository;
            _departmentRespository = departmentRespository;
        }
        #endregion

        #region Methods
        public virtual void DeleteEmployee(Employee employeeRecord)
        {
            if (employeeRecord == null)
                throw new ArgumentNullException("employeeRecord");

            _employeeRepository.Delete(employeeRecord);

            _cacheManager.RemoveByPrefix(EMPLOYEES_PATTERN_KEY);
        }

        public virtual IPagedList<Employee> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(EMPLOYEES_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from employee in _employeeRepository.Table
                            where employee.Deleted == false && employee.Published
                            orderby employee.Id
                            select employee;

                var records = new PagedList<Employee>(query, pageIndex, pageSize);
                return records;
            });
        }

        public virtual Employee GetById(int id)
        {
            if (id == 0)
                return null;

            return _employeeRepository.GetById(id);
        }

        public virtual Employee GetByEmailPrefix(string emailPrefix)
        {
            if (string.IsNullOrWhiteSpace(emailPrefix))
                return null;

            return _employeeRepository
                .Table
                .Where(x => !string.IsNullOrWhiteSpace(x.Email) && x.Email.ToLower().StartsWith(emailPrefix + '@'))
                .FirstOrDefault();
        }

        public virtual void InsertEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");

            ValidateEmployee(employee);

            _employeeRepository.Insert(employee);

            _cacheManager.RemoveByPrefix(EMPLOYEES_PATTERN_KEY);
        }

        private void ValidateEmployee(Employee employee)
        {
            if (!employee.WorkStarted.HasValue || employee.WorkStarted.Value.Year < 1901)
                employee.WorkStarted = new DateTime(1901, 1, 1);
            if (!employee.WorkEnded.HasValue || employee.WorkEnded.Value.Year < 1901)
                employee.WorkEnded = new DateTime(1901, 1, 1);
        }

        public virtual void InsertDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException("department");

            _departmentRespository.Insert(department);

            _cacheManager.RemoveByPrefix(DEPARTMENTS_ALL_KEY);
        }

        public virtual void UpdateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employeeRecord");

            ValidateEmployee(employee);

            _employeeRepository.Update(employee);

            _cacheManager.RemoveByPrefix(EMPLOYEES_PATTERN_KEY);
        }

        public virtual void UpdateDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException("departmentRecord");

            _departmentRespository.Update(department);

            _cacheManager.RemoveByPrefix(DEPARTMENTS_ALL_KEY);
        }

        public virtual IList<Department> GetAllDepartments(bool showHidden = false)
        {
            string key = string.Format(DEPARTMENTS_ALL_KEY,showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from department in _departmentRespository.Table
                            where department.Published || showHidden
                            orderby department.DisplayOrder
                            select department;

                var records = query.ToList();
                return records;
            });
        }

        public virtual IList<Employee> GetEmployeesByDepartmentId(int departId)
        {
            var query = from employee in _employeeRepository.Table
                        where employee.Published && employee.DepartmentId == departId && !employee.Deleted
                        orderby employee.Name
                        select employee;

            var records = query.ToList();
            return records;
        }

        public virtual Department GetDepartmentById(int departId)
        {
            var query = from sbw in _departmentRespository.Table
                        where sbw.Id == departId
                        orderby sbw.Name
                        select sbw;

            var record = query.FirstOrDefault();
            return record;
        }
        #endregion
    }
}
