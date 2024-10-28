using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Interface
{
    public interface ITokenService
    {
        string GenerateToken(IdentityUser user);
    }
}
