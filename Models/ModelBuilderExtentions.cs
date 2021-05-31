using Microsoft.EntityFrameworkCore;

namespace EmployeeManagment.Models
{
    public static class ModelBuilderExtentions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 3,
                    Department = Dept.HR,
                    Name = "john",
                    Email = "john@gmail.com"
                },
                 new Employee
                 {
                     Id = 4,
                     Department = Dept.Payroll,
                     Name = "amir",
                     Email = "air@gmail.com"
                 }
                );
        }
    }
}
