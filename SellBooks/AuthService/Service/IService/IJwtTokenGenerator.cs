using SellBooks.Models;

namespace SellBooks.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string>roles);
    }
}
