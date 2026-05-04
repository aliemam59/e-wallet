using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test_api_jwt.Data;
using test_api_jwt.Models;

namespace test_api_jwt.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TransferAsync(int senderId, string receiverUsername, decimal amount)
        {
            if (amount <= 0) return false;

            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == senderId);
            var receiver = await _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Username == receiverUsername);

            if (senderWallet == null || receiver == null || receiver.Wallet == null)
                return false;

            if (senderWallet.Balance < amount)
                return false; // الرصيد غير كافي

            // خصم من المرسل
            senderWallet.Balance -= amount;
            _context.Transactions.Add(new Transaction
            {
                Amount = amount,
                Type = "Transfer_Out",
                WalletId = senderWallet.Id
            });

            // إضافة للمستقبل
            receiver.Wallet.Balance += amount;
            _context.Transactions.Add(new Transaction
            {
                Amount = amount,
                Type = "Transfer_In",
                WalletId = receiver.Wallet.Id
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null) return new List<Transaction>();

            return await _context.Transactions
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(t => t.DateTime)
                .ToListAsync();
        }
    }
}