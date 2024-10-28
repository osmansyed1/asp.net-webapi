namespace EmployeePortal.Models
{
    public class FacebookValidationResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsValid => !string.IsNullOrEmpty(Id);
    }
}
