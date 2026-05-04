namespace test_api_jwt.DTOs.Transfer
{
    public class TransferDto
    {
        public string ReceiverUsername { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}