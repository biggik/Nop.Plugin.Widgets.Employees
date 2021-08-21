using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Employees.Domain;
using System.Threading.Tasks;
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
#if !NOP_ASYNC
        private readonly ICacheKeyService _cacheKeyService;
#endif
        private readonly IStaticCacheManager _cacheManager;
#endif
#endregion

        #region Ctor
        public EmployeesService(
#if NOP_PRE_4_3
            ICacheManager cacheManager,
#else
#if !NOP_ASYNC
            ICacheKeyService cacheKeyService,
#endif
            IStaticCacheManager cacheManager,
#endif
            IRepository<Employee> employeeRepository,
            IRepository<Department> departmentRespository)
        {
            _cacheManager = cacheManager;
#if !NOP_ASYNC
#if !NOP_PRE_4_3
            _cacheKeyService = cacheKeyService;
#endif
#endif
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRespository;
        }
        #endregion

#if NOP_ASYNC
        private IStaticCacheManager CacheImpl => _cacheManager;
#elif !NOP_PRE_4_3
        private ICacheKeyService CacheImpl => _cacheKeyService;
#endif

#if NOP_PRE_4_3
        private string CreateKey(string template, params object[] arguments)
        {
            return string.Format(template, arguments);
        }
#else
        private CacheKey CreateKey(CacheKey cacheKey, params object[] arguments)
        {
            return CacheImpl.PrepareKeyForShortTermCache(cacheKey, arguments);
        }
#endif

#region Methods
#if NOP_ASYNC
        public virtual async Task DeleteEmployeeAsync(Employee employee)
#else
        public virtual void DeleteEmployee(Employee employee)
#endif
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

#if NOP_ASYNC
            await _employeeRepository.DeleteAsync(employee);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
#else
            _employeeRepository.Delete(employee);
            _cacheManager.RemoveByPrefix(_prefix);
#endif
        }

#if NOP_ASYNC
        public virtual async Task DeleteDepartmentAsync(Department department)
#else
        public virtual void DeleteDepartment(Department department)
#endif
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

#if NOP_ASYNC
            await _departmentRepository.DeleteAsync(department);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
#else
            _departmentRepository.Delete(department);
            _cacheManager.RemoveByPrefix(_prefix);
#endif
        }

#if NOP_ASYNC
        public virtual async Task<IPagedList<Employee>> GetOrderedEmployeesAsync(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(EmployeesAllKey, showUnpublished, pageIndex, pageSize);
            return await _cacheManager.GetAsync(key, () =>
            {
                return _employeeRepository.GetAllPagedAsync(query =>
                {
                    return from employee in query
                           join department in _departmentRepository.Table on employee.DepartmentId equals department.Id
                           where showUnpublished
                                 ||
                                 (employee.Published
                                 && (!employee.WorkStarted.HasValue || employee.WorkStarted.Value.Date < DateTime.Now.Date)
                                 && (!employee.WorkEnded.HasValue ||
                                         (employee.WorkEnded.Value.Year < 2000 || employee.WorkEnded.Value > DateTime.Now.Date)
                                 ))
                           orderby department.DisplayOrder ascending,
                                   department.Name ascending,
                                   employee.DisplayOrder ascending,
                                   employee.Name ascending

                           select employee;
                }, pageIndex, pageSize
                );
            });
        }
#else
        public virtual IPagedList<Employee> GetOrderedEmployees(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(EmployeesAllKey, showUnpublished, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from employee in _employeeRepository.Table
                            join department in _departmentRepository.Table on employee.DepartmentId equals department.Id
                            where showUnpublished
                                  ||
                                  (employee.Published
                                  && (!employee.WorkStarted.HasValue || employee.WorkStarted.Value.Date < DateTime.Now.Date)
                                  && (!employee.WorkEnded.HasValue ||
                                          (employee.WorkEnded.Value.Year < 2000 || employee.WorkEnded.Value > DateTime.Now.Date)
                                  ))
                            orderby department.DisplayOrder ascending,
                                    department.Name ascending,
                                    employee.DisplayOrder ascending,
                                    employee.Name ascending

                            select employee;

                return new PagedList<Employee>(
                    query,    
                    pageIndex,
                    pageSize);
            });
        }
#endif

#if NOP_ASYNC
        public virtual async Task<Employee> GetByIdAsync(int id)
        {
            if (id == 0)
                return null;

            return await _employeeRepository.GetByIdAsync(id);
        }
#else
        public virtual Employee GetById(int id)
        {
            if (id == 0)
                return null;

            return _employeeRepository.GetById(id);
        }
#endif

#if NOP_ASYNC
        public virtual async Task<Employee> GetByEmailPrefixAsync(string emailPrefix)
#else
        public virtual Employee GetByEmailPrefix(string emailPrefix)
#endif
        {
            if (string.IsNullOrWhiteSpace(emailPrefix))
                return null;

#if NOP_ASYNC
            return (await _employeeRepository.GetAllAsync(query =>
            {
                return from employee in query
#else
            return (from employee in _employeeRepository.Table
#endif
                       where employee.Published
                             && !(employee.Email == null || employee.Email.Trim() == string.Empty)
                             && employee.Email.ToLower().StartsWith(emailPrefix.ToLower() + '@')
                       select employee
#if NOP_ASYNC
                    ;
            }))
#else
                   )
#endif
                   .FirstOrDefault();
        }

#if NOP_ASYNC
        public virtual async Task InsertEmployeeAsync(Employee employee)
#else
        public virtual void InsertEmployee(Employee employee)
#endif
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

#if NOP_ASYNC
            await _employeeRepository.InsertAsync(employee);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
#else
            _employeeRepository.Insert(employee);
            _cacheManager.RemoveByPrefix(_prefix);
#endif
        }

#if NOP_ASYNC
        public virtual async Task<IPagedList<Department>> GetOrderedDepartmentsAsync(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(DepartmentsKey, showUnpublished, pageIndex, pageSize);
            return await _cacheManager.GetAsync(key, () =>
            {
                return _departmentRepository.GetAllPagedAsync(query =>
                {
                    return from department in query
                           where department.Published || showUnpublished

                           orderby department.DisplayOrder,
                                   department.Name

                           select department;
                },
                pageIndex,
                pageSize);
            });
        }
#else
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
#endif

#if NOP_ASYNC
        public virtual async Task InsertDepartmentAsync(Department department)
#else
        public virtual void InsertDepartment(Department department)
#endif
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

#if NOP_ASYNC
            await _departmentRepository.InsertAsync(department);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
#else
            _departmentRepository.Insert(department);
            _cacheManager.RemoveByPrefix(_prefix);
#endif
        }

#if NOP_ASYNC
        public virtual async Task UpdateEmployeeAsync(Employee employee)
#else
        public virtual void UpdateEmployee(Employee employee)
#endif
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

#if NOP_ASYNC
            await _employeeRepository.UpdateAsync(employee);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
#else
            _employeeRepository.Update(employee);
            _cacheManager.RemoveByPrefix(_prefix);
#endif        
        }

#if NOP_ASYNC
        public virtual async Task UpdateDepartmentAsync(Department department)
#else
        public virtual void UpdateDepartment(Department department)
#endif
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

#if NOP_ASYNC
            await _departmentRepository.UpdateAsync(department);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
#else
            _departmentRepository.Update(department);
            _cacheManager.RemoveByPrefix(_prefix);
#endif
        }

#if NOP_ASYNC
        public virtual async Task<IList<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId, bool showUnpublished = false)
        {
            var key = CreateKey(EmployeesDepartmentKey, departmentId, showUnpublished);
            return await _cacheManager.GetAsync(key, () =>
            {
                return _employeeRepository.GetAllAsync(query =>
                {
                    return from employee in query
                           where employee.DepartmentId == departmentId
                                 && (showUnpublished
                                     ||
                                     (employee.Published
                                      && (!employee.WorkStarted.HasValue || employee.WorkStarted.Value.Date < DateTime.Now.Date)
                                      && (!employee.WorkEnded.HasValue ||
                                              (employee.WorkEnded.Value.Year < 2000 || employee.WorkEnded.Value > DateTime.Now.Date)
                                         )))

                           orderby employee.DisplayOrder, employee.Name

                           select employee;
                });
            });
        }
#else
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
                                   && (!employee.WorkStarted.HasValue || employee.WorkStarted.Value.Date < DateTime.Now.Date)
                                   && (!employee.WorkEnded.HasValue ||
                                           (employee.WorkEnded.Value.Year < 2000 || employee.WorkEnded.Value > DateTime.Now.Date)
                                      )))
                        
                        orderby employee.DisplayOrder, employee.Name
                        
                        select employee).ToList();
            });
        }
#endif

#if NOP_ASYNC
        public virtual async Task<Department> GetDepartmentByIdAsync(int departmentId)
        {
            return await _departmentRepository.GetByIdAsync(departmentId, cache => default);
        }
#else
        public virtual Department GetDepartmentById(int departmentId)
        {
            return (from department in _departmentRepository.Table
                    where department.Id == departmentId
                    select department)
                    .FirstOrDefault();
        }
#endif
        #endregion
    }
}
