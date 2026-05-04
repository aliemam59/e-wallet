using System.Collections.Generic;
using System.Threading.Tasks;
using test_api_jwt.Models;

namespace test_api_jwt.Services
{
    public interface ITransactionService
    {
        Task<bool> TransferAsync(int senderId, string receiverUsername, decimal amount);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
    }
}