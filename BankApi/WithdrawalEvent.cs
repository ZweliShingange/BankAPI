using System.Text.Json;

namespace BankApi
{
    public class WithdrawalEvent
    {
        public decimal Amount { get; }
        public string AccountId { get; }
        public string Status { get; }

        public WithdrawalEvent(decimal amount, string accountId, string status)
        {
            Amount = amount;
            AccountId = accountId;
            Status = status;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
