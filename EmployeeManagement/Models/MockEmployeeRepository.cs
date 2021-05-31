using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    // Its important to use Interface instead of direct classes to allow dependency injection
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Swapnil", Department = "Engineering", Email = "blah.blah@yahoo1.com" },
                new Employee() { Id = 2, Name = "John", Department = "HR", Email = "john.doe@gmail.com" },
                new Employee() { Id = 3, Name = "Ashlee", Department = "Sales", Email = "blah.blah1@hotmail.com" }
            };
        }
        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }
    }
}
