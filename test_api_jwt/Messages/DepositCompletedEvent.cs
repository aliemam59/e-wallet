namespace test_api_jwt.Events
{
    // بنستخدم record بدل class لأنه أسرع ومناسب جداً للرسايل
    public record DepositCompletedEvent(int UserId, decimal Amount, string Message);
}