namespace EmployeePortal.Models
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        public string Role { get; set; } // Role to be assigned
    }

}
