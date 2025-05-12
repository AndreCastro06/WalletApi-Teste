namespace WalletApi.Services
{
    using WalletApi.Models;

    public interface IWalletService
    {
        Task<decimal> ConsultarSaldoAsync(int userId);
        Task<bool> AdicionarSaldoAsync(int userId, decimal valor);
        Task<bool> TransferirAsync(int deUserId, int paraUserId, decimal valor);
        Task<List<Transaction>> ListarTransacoesAsync(int userId, DateTime? inicio = null, DateTime? fim = null);
    }
}