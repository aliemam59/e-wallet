using System.Threading.Tasks;

namespace test_api_jwt.Services
{
    public interface IWalletService
    {
        Task<decimal> GetBalanceAsync(int userId);
        Task DepositAsync(int userId, decimal amount);
        Task<bool> WithdrawAsync(int userId, decimal amount);
    }
}