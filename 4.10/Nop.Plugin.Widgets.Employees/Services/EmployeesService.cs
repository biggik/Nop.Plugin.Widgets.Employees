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
        private readonly IRepository<EmployeesRecord> _sbwRepository;
        private readonly IRepository<DepartmentRecord> _departRespository;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Ctor
        public EmployeesService(ICacheManager cacheManager,
            IRepository<EmployeesRecord> sbwRepository, IRepository<DepartmentRecord> departRespository)
        {
            this._cacheManager = cacheManager;
            this._sbwRepository = sbwRepository;
            this._departRespository = departRespository;
        }
        #endregion

        #region Methods
        public virtual void DeleteEmployeesRecord(EmployeesRecord employeeRecord)
        {
            if (employeeRecord == null)
                throw new ArgumentNullException("employeeRecord");

            _sbwRepository.Delete(employeeRecord);

            _cacheManager.RemoveByPattern(EMPLOYEES_PATTERN_KEY);
        }

        public virtual IPagedList<EmployeesRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(EMPLOYEES_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbw in _sbwRepository.Table
                            where sbw.Deleted == false && sbw.Published
                            orderby sbw.Id
                            select sbw;

                var records = new PagedList<EmployeesRecord>(query, pageIndex, pageSize);
                return records;
            });
        }

        public virtual EmployeesRecord GetById(int id)
        {
            if (id == 0)
                return null;

            return _sbwRepository.GetById(id);
        }

        public virtual void InsertEmployeesRecord(EmployeesRecord employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");

            ValidateEmployee(employee);

            _sbwRepository.Insert(employee);

            _cacheManager.RemoveByPattern(EMPLOYEES_PATTERN_KEY);
        }

        private void ValidateEmployee(EmployeesRecord employee)
        {
            if (!employee.WorkStarted.HasValue || employee.WorkStarted.Value.Year < 1901)
                employee.WorkStarted = new DateTime(1901, 1, 1);
            if (!employee.WorkEnded.HasValue || employee.WorkEnded.Value.Year < 1901)
                employee.WorkEnded = new DateTime(1901, 1, 1);
        }

        public virtual void InsertDepartmentRecord(DepartmentRecord department)
        {
            if (department == null)
                throw new ArgumentNullException("department");

            _departRespository.Insert(department);

            _cacheManager.RemoveByPattern(DEPARTMENTS_ALL_KEY);
        }

        public virtual void UpdateEmployeesRecord(EmployeesRecord employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employeeRecord");

            ValidateEmployee(employee);

            _sbwRepository.Update(employee);

            _cacheManager.RemoveByPattern(EMPLOYEES_PATTERN_KEY);
        }

        public virtual void UpdateDepartmentRecord(DepartmentRecord department)
        {
            if (department == null)
                throw new ArgumentNullException("departmentRecord");

            _departRespository.Update(department);

            _cacheManager.RemoveByPattern(DEPARTMENTS_ALL_KEY);
        }

        public virtual IList<DepartmentRecord> GetAllDepartments(bool showHidden = false)
        {
            string key = string.Format(DEPARTMENTS_ALL_KEY,showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbw in _departRespository.Table
                            where sbw.Published || showHidden
                            orderby sbw.DisplayOrder
                            select sbw;

                var records = query.ToList();
                return records;
            });
        }

        public virtual IList<EmployeesRecord> GetEmployeeByDepartmentId(int departId)
        {
            var query = from sbw in _sbwRepository.Table
                        where sbw.Published && sbw.DepartmentId == departId && sbw.Deleted == false
                        orderby sbw.Name
                        select sbw;

            var records = query.ToList();
            return records;
        }

        public virtual DepartmentRecord GetDepartmentByDepartmentId(int departId)
        {
            var query = from sbw in _departRespository.Table
                        where sbw.Id == departId
                        orderby sbw.Name
                        select sbw;

            var record = query.FirstOrDefault();
            return record;
        }
        #endregion
    }
}
