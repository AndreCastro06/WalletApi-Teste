namespace WalletApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int DeUserId { get; set; }
        public int ParaUserId { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
    }
}