using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Employees.Domain;
#if NOP_PRE_4_3
using Nop.Core.Data;
#else
using Nop.Data;
using Nop.Services.Caching;
#endif

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial class EmployeesService : IEmployeesService
    {
        #region Constants
        private const string _prefix = "Nop.status.employee.";
        private readonly static string _allKey = _prefix + "all-{0}-{1}-{2}";
        private readonly static string _departmentKey = _prefix + "department.all-{0}-{1}";
        private readonly static string _departmentsKey = _prefix + "departments.all-{0}-{1}-{2}";

#if NOP_PRE_4_3
        private readonly static string EmployeesAllKey = _allKey;
        private readonly static string EmployeesDepartmentKey = _departmentKey;
        private readonly static string DepartmentsKey = _departmentsKey;
#else
        private readonly CacheKey EmployeesAllKey = new CacheKey(_allKey, _prefix);
        private readonly CacheKey EmployeesDepartmentKey = new CacheKey(_departmentKey, _prefix);
        private readonly CacheKey DepartmentsKey = new CacheKey(_departmentsKey, _prefix);
#endif
        #endregion

        #region Fields
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRepository;
#if NOP_PRE_4_3
        private readonly ICacheManager _cacheManager;
#else
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _cacheManager;
#endif
        #endregion

        #region Ctor
        public EmployeesService(
#if NOP_PRE_4_3
            ICacheManager cacheManager,
#else
            ICacheKeyService cacheKeyService,
            IStaticCacheManager cacheManager,
#endif
            IRepository<Employee> employeeRepository,
            IRepository<Department> departmentRespository)
        {
            _cacheManager = cacheManager;
#if !NOP_PRE_4_3
            _cacheKeyService = cacheKeyService;
#endif
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRespository;
        }
        #endregion

#if NOP_PRE_4_3
        private string CreateKey(string template, params object[] arguments)
        {
            return string.Format(template, arguments);
        }
#else
        private CacheKey CreateKey(CacheKey cacheKey, params object[] arguments)
        {
            return _cacheKeyService.PrepareKeyForShortTermCache(cacheKey, arguments);
        }
#endif

        #region Methods
        public virtual void DeleteEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employeeRepository.Delete(employee);

            _cacheManager.RemoveByPrefix(_prefix);
        }

        public virtual void DeleteDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            _departmentRepository.Delete(department);

            _cacheManager.RemoveByPrefix(_prefix);
        }

        public virtual IPagedList<Employee> GetOrderedEmployees(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(EmployeesAllKey, showUnpublished, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                return new PagedList<Employee>(
                    from employee in _employeeRepository.Table
                    join department in _departmentRepository.Table on employee.DepartmentId equals department.Id 
                    where showUnpublished
                          ||
                          (employee.Published
                          && (employee.WorkStarted.Date < DateTime.Now.Date)
                          && (!employee.WorkEnded.HasValue ||
                                  (employee.WorkEnded.Value.Year < 2000 || employee.WorkEnded.Value > DateTime.Now.Date)
                          ))
                    
                    orderby department.DisplayOrder ascending,
                            department.Name ascending,
                            employee.DisplayOrder ascending,
                            employee.Name ascending

                    select employee,
                    pageIndex,
                    pageSize);
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

            return (from employee in _employeeRepository.Table
                    where employee.Published
                          && !string.IsNullOrWhiteSpace(employee.Email)
                          && employee.Email.ToLower().StartsWith(emailPrefix.ToLower() + '@')
                    select employee
                   ).FirstOrDefault();
        }

        public virtual void InsertEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employeeRepository.Insert(employee);

            _cacheManager.RemoveByPrefix(_prefix);
        }

        public virtual IPagedList<Department> GetOrderedDepartments(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(DepartmentsKey, showUnpublished, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                return new PagedList<Department>(
                    from department in _departmentRepository.Table
                    where department.Published || showUnpublished
                    
                    orderby department.DisplayOrder,
                            department.Name
                    
                    select department,
                    pageIndex,
                    pageSize);
            });
        }

        public virtual void InsertDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            _departmentRepository.Insert(department);

            _cacheManager.RemoveByPrefix(_prefix);
        }

        public virtual void UpdateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employeeRepository.Update(employee);

            _cacheManager.RemoveByPrefix(_prefix);
        }

        public virtual void UpdateDepartment(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            _departmentRepository.Update(department);

            _cacheManager.RemoveByPrefix(_prefix);
        }

        public virtual IList<Employee> GetEmployeesByDepartmentId(int departmentId, bool showUnpublished = false)
        {
            var key = CreateKey(EmployeesDepartmentKey, departmentId, showUnpublished);
            return _cacheManager.Get(key, () =>
            {
                return (from employee in _employeeRepository.Table
                        where employee.DepartmentId == departmentId
                              && (showUnpublished
                                  ||
                                  (employee.Published
                                   && (employee.WorkStarted.Date < DateTime.Now.Date)
                                   && (!employee.WorkEnded.HasValue ||
                                           (employee.WorkEnded.Value.Year < 2000 || employee.WorkEnded.Value > DateTime.Now.Date)
                                      )))
                        
                        orderby employee.DisplayOrder, employee.Name
                        
                        select employee).ToList();
            });
        }

        public virtual Department GetDepartmentById(int departmentId)
        {
            return (from department in _departmentRepository.Table
                    where department.Id == departmentId
                    select department)
                    .FirstOrDefault();
        }
        #endregion
    }
}
