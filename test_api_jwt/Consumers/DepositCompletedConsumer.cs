using MassTransit;
using Microsoft.Extensions.Logging;
using test_api_jwt.Events;

namespace test_api_jwt.Consumers
{
    public class DepositCompletedConsumer : IConsumer<DepositCompletedEvent>
    {
        private readonly ILogger<DepositCompletedConsumer> _logger;

        public DepositCompletedConsumer(ILogger<DepositCompletedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DepositCompletedEvent> context)
        {
            var data = context.Message;

            // هنا بنعمل محاكاة لعملية بتاخد وقت (زي إرسال إيميل أو SMS)
            _logger.LogInformation($"[Background Task] Started sending email to User {data.UserId}...");

            await Task.Delay(3000); // بنعطله 3 ثواني وهمية

            _logger.LogInformation($"[Background Task] ✅ Email sent successfully! Message: {data.Message}, Amount: {data.Amount}");
        }
    }
}