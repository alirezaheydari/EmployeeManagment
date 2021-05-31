using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private static List<Employee> _employeeList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Ghazal", Department = Dept.HR , Email = "Ghazal@gmail.com" },
                new Employee() { Id = 2, Name = "Raha", Department = Dept.IT , Email = "Raha@gmail.com" },
                new Employee() { Id = 3, Name = "Mina", Department = Dept.IT , Email = "Mina@gmail.com" }
            };

        public Employee add(Employee employee)
        {
            employee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = _employeeList.FirstOrDefault(x => x.Id == id);
            if (employee == null)
                _employeeList.Remove(employee);
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(x => x.Id.Equals(id));
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = _employeeList.FirstOrDefault(x => x.Id == employeeChanges.Id);
            if (employee == null)
                employee = employeeChanges;
            return employee;
        }
    }
}
