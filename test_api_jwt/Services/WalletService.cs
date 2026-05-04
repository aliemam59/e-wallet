using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed; // <-- مكتبة الـ Redis اللي نزلناها
using System;
using System.Threading.Tasks;
using test_api_jwt.Data;
using test_api_jwt.Models;
using MassTransit;
using test_api_jwt.Events;
using MassTransit.Transports;
namespace test_api_jwt.Services
{
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache; // <-- تعريف الكاش
        private readonly IPublishEndpoint _publishEndpoint;
        // حقن الكاش مع الداتا بيز
        public WalletService(AppDbContext context, IDistributedCache cache, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _cache = cache;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            string cacheKey = $"balance_user_{userId}";

            // 1. ندور في الريديس الأول (سريع جداً)
            var cachedBalance = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedBalance))
            {
                return decimal.Parse(cachedBalance); // لقيناه! رجعه فوراً
            }

            // 2. ملقيناهوش؟ نروح للـ SQL Server (أبطأ شوية)
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            decimal balance = wallet?.Balance ?? 0;

            // 3. نحفظ نسخة في الريديس عشان المرة الجاية (صلاحية 5 دقايق)
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(cacheKey, balance.ToString(), options);

            return balance;
        }

        public async Task DepositAsync(int userId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null) return;

            wallet.Balance += amount;

            _context.Transactions.Add(new Transaction
            {
                Amount = amount,
                Type = "Deposit",
                WalletId = wallet.Id
            });

            await _context.SaveChangesAsync();

            // 4. (هام جداً) نمسح الرصيد القديم من الكاش عشان اتغير
            await _cache.RemoveAsync($"balance_user_{userId}");

            await _publishEndpoint.Publish(new DepositCompletedEvent(userId, amount, "Deposit Successful. Thank you!"));
        }

        public async Task<bool> WithdrawAsync(int userId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null || wallet.Balance < amount) return false;

            wallet.Balance -= amount;

            _context.Transactions.Add(new Transaction
            {
                Amount = amount,
                Type = "Withdraw",
                WalletId = wallet.Id
            });

            await _context.SaveChangesAsync();

            // 5. (هام جداً) نمسح الرصيد القديم من الكاش عشان اتغير
            await _cache.RemoveAsync($"balance_user_{userId}");

            return true;
        }
    }
}