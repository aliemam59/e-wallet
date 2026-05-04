using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using test_api_jwt.Services;
using test_api_jwt.DTOs.Wallet;

namespace test_api_jwt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var balance = await _walletService.GetBalanceAsync(userId);
            return Ok(new { balance });
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _walletService.DepositAsync(userId, dto.Amount);
            return Ok(new { message = "Deposit successful" });
        }
    }
}