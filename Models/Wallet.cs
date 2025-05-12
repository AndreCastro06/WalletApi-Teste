namespace WalletApi.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public decimal Saldo { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}