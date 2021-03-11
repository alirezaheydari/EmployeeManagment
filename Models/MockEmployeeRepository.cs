using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Ghazal", Department = "HR" , Email = "Ghazal@gmail.com" },
                new Employee() { Id = 2, Name = "Raha", Department = "IT" , Email = "Raha@gmail.com" },
                new Employee() { Id = 3, Name = "Mina", Department = "IT" , Email = "Mina@gmail.com" }
            };
        }
        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(x => x.Id.Equals(id));
        }
    }
}
