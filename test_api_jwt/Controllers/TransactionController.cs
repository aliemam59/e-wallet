using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using test_api_jwt.Services;
using test_api_jwt.DTOs.Transfer;

namespace test_api_jwt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var success = await _transactionService.TransferAsync(senderId, dto.ReceiverUsername, dto.Amount);

            if (!success) return BadRequest(new { message = "Transfer failed. Check balance or receiver username." });

            return Ok(new { message = "Transfer successful" });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = await _transactionService.GetUserTransactionsAsync(userId);

            return Ok(transactions);
        }
    }
}