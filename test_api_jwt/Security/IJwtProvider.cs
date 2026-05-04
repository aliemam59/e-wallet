using test_api_jwt.Models;

namespace test_api_jwt.Security
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}