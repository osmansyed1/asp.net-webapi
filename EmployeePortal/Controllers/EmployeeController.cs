using EmployeePortal.Models;
using EmployeePortal.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepository _repository;

        public EmployeeController(EmployeeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> getEmployeeList()
        {
            var allEmp= await _repository.GetAllEmployee();
            return Ok(allEmp);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> addEmployee([FromBody]Employee vm)
        {
            /* await _repository.saveEmployee(vm);
             return Ok(vm);*/
         

            await _repository.saveEmployee(vm);
            return CreatedAtAction(nameof(getEmployeeList), new { id = vm.Id }, vm);

            /*  return Ok(vm);*/
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> updateEmployee(string id, [FromBody]Employee employee)
        {
            await _repository.updateEmployee(id, employee);
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        [Authorize] 
        public async Task<ActionResult> deleteEmployee(string id)
        {
            await _repository.deleteEmployee(id);
            return Ok();    
        }
        
    }
}
