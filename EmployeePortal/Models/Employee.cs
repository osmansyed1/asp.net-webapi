using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.Models
{
    public class Employee
    {
        [Key]
        public string Id { get; set; }= Guid.NewGuid().ToString();
        public  required string Name { get; set; }

        public required string Email { get; set; }

        public required int Mobile { get; set; }

        public required string Department { get; set; } 
        
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Gender { get; set; }

        public int? Salary { get; set; }  

        public bool? Status {  get; set; }
     /*   [ForeignKey("User")]
    *//*    public string UserId { get; set; }

        public virtual IdentityUser User { get; set; }*/

    }
}
    