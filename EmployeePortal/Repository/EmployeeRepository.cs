using EmployeePortal.Data;
using EmployeePortal.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EmployeePortal.Repository
{
    public class EmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Employee>> GetAllEmployee()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task saveEmployee(Employee emp)
        {
            await _context.Employees.AddAsync(emp);
            await _context.SaveChangesAsync();  
        }

        public async Task updateEmployee(string id,Employee emp)
        {
            var employee=await _context.Employees.FindAsync(id);
            if(employee == null)
            {
                throw new Exception("Employee not found");

            }
            employee.Name = emp.Name;
            employee.Email = emp.Email;
            employee.Mobile = emp.Mobile;   
            employee.City = emp.City;
            employee.Gender = emp.Gender;   
            employee.Department = emp.Department;
            employee.Status = emp.Status;
            employee.Salary = emp.Salary;
            employee.Address = emp.Address;

            await _context.SaveChangesAsync();
            
        }

        public async Task deleteEmployee(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if(employee == null)
            {
                throw new Exception("Employee not found");
                     
            }
             _context.Remove(employee);
            await _context.SaveChangesAsync();
        }

    }
}
