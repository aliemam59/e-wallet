using System;

namespace test_api_jwt.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; 
        public DateTime DateTime { get; set; } = DateTime.Now;

        public int WalletId { get; set; }
        public Wallet? Wallet { get; set; }
    }
}