using EmployeeManagment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModel
{
    public class EmployeeEditViewModel : EmployeeCreateViewModel
    {
        public EmployeeEditViewModel()
        {

        }
        public EmployeeEditViewModel(Employee employee)
        {
            Id = employee.Id;
            Name = employee.Name;
            Department = employee.Department;
            Email = employee.Email;
            ExistingPhotoPath = employee.PhotoPath;
        }
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
