using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Threading.Tasks;
using test_api_jwt.Data;
using test_api_jwt.Models;
using test_api_jwt.Services;
using Xunit;

namespace test_api_jwt.Tests
{
    public class WalletServiceTests
    {
        [Fact]
        public async Task WithdrawAsync_ShouldReturnFalse_WhenInsufficientFunds()
        {
            // 1. Arrange (تجهيز الدفاتر الوهمية للمراجعة)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestBankDb") // داتا بيز وهمية في الرامات
                .Options;

            using var context = new AppDbContext(options);

            // بنحط عميل رقم 1 ورصيده 100 بس
            context.Wallets.Add(new Wallet { UserId = 1, Balance = 100 });
            await context.SaveChangesAsync();

            // بنعمل "درج كاش" وهمي عشان الـ Service متزعلش
            var mockCache = new Mock<IDistributedCache>();

            // بنشغل الخدمة بتاعتنا بالداتا الوهمية
            var service = new WalletService(context, mockCache.Object);

            // 2. Act (تنفيذ العملية المراد فحصها)
            // العميل بيحاول يسحب 150 (أكبر من رصيده)
            var result = await service.WithdrawAsync(userId: 1, amount: 150m);

            // 3. Assert (التأكد من النتيجة - Audit Verification)
            // بنقول للسيستم: "أنا بأكد إن النتيجة لازم تطلع False، لو طلعت True يبقى في اختلاس أو ثغرة"
            Assert.False(result);
        }
    }
}