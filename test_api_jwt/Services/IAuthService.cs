using System.Threading.Tasks;
using test_api_jwt.DTOs.Auth;

namespace test_api_jwt.Services
{
    public interface  IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}