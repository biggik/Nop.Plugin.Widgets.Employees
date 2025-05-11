using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Employees.Domain;
using System.Threading.Tasks;
using Nop.Data;

namespace Nop.Plugin.Widgets.Employees.Services
{
    public partial class EmployeesService : IEmployeesService
    {
        #region Constants
        private const string _prefix = "Nop.status.employee.";
        private readonly static string _allKey = _prefix + "all-{0}-{1}-{2}";
        private readonly static string _departmentKey = _prefix + "department.all-{0}-{1}";
        private readonly static string _departmentsKey = _prefix + "departments.all-{0}-{1}-{2}";

        private readonly CacheKey EmployeesAllKey = new CacheKey(_allKey, _prefix);
        private readonly CacheKey EmployeesDepartmentKey = new CacheKey(_departmentKey, _prefix);
        private readonly CacheKey DepartmentsKey = new CacheKey(_departmentsKey, _prefix);
        #endregion

        #region Fields
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IStaticCacheManager _cacheManager;
        #endregion

        #region Ctor
        public EmployeesService(
            IStaticCacheManager cacheManager,
            IRepository<Employee> employeeRepository,
            IRepository<Department> departmentRespository)
        {
            _cacheManager = cacheManager;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRespository;
        }
        #endregion

        private IStaticCacheManager CacheImpl => _cacheManager;

        private CacheKey CreateKey(CacheKey cacheKey, params object[] arguments)
        {
            return CacheImpl.PrepareKeyForDefaultCache(cacheKey, arguments);
        }

        #region Methods
        public virtual async Task DeleteEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _employeeRepository.DeleteAsync(employee);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
        }

        public virtual async Task DeleteDepartmentAsync(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            await _departmentRepository.DeleteAsync(department);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
        }

        public virtual Task<IPagedList<Employee>> GetOrderedEmployeesAsync(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(EmployeesAllKey, showUnpublished, pageIndex, pageSize);
            return _cacheManager.GetAsync(key, () =>
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

        public virtual async Task<Employee> GetByIdAsync(int id)
        {
            if (id == 0)
                return null;

            return await _employeeRepository.GetByIdAsync(id);
        }

        public virtual async Task<Employee> GetByEmailPrefixAsync(string emailPrefix)
        {
            if (string.IsNullOrWhiteSpace(emailPrefix))
                return null;

            return (await _employeeRepository.GetAllAsync(query =>
            {
                return from employee in query
                       where employee.Published
                             && !(employee.Email == null || employee.Email.Trim() == string.Empty)
                             && employee.Email.ToLower().StartsWith(emailPrefix.ToLower() + '@')
                       select employee
                    ;
            }))
            .FirstOrDefault();
        }

        public virtual async Task InsertEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _employeeRepository.InsertAsync(employee);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
        }

        public virtual Task<IPagedList<Department>> GetOrderedDepartmentsAsync(bool showUnpublished, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = CreateKey(DepartmentsKey, showUnpublished, pageIndex, pageSize);
            return _cacheManager.GetAsync(key, () =>
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

        public virtual async Task InsertDepartmentAsync(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            await _departmentRepository.InsertAsync(department);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
        }

        public virtual async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _employeeRepository.UpdateAsync(employee);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
        }

        public virtual async Task UpdateDepartmentAsync(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            await _departmentRepository.UpdateAsync(department);
            await _cacheManager.RemoveByPrefixAsync(_prefix);
        }

        public virtual Task<IList<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId, bool showUnpublished = false)
        {
            var key = CreateKey(EmployeesDepartmentKey, departmentId, showUnpublished);
            return _cacheManager.GetAsync(key, () =>
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

        public virtual Task<Department> GetDepartmentByIdAsync(int departmentId)
        {
            return _departmentRepository.GetByIdAsync(departmentId, cache => default);
        }
        #endregion
    }
}
