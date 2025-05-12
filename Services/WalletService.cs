namespace WalletApi.Services
{
    using Microsoft.EntityFrameworkCore;
    using WalletApi.Data;
    using WalletApi.Models;
    using WalletApi.Services;


    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;

        public WalletService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> ConsultarSaldoAsync(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.Saldo ?? 0;
        }


        public async Task<bool> TransferirAsync(int deUserId, int paraUserId, decimal valor)
        {
            if (valor <= 0 || deUserId == paraUserId)
                return false;

            var carteiraOrigem = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == deUserId);
            var carteiraDestino = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == paraUserId);

            if (carteiraOrigem == null || carteiraDestino == null || carteiraOrigem.Saldo < valor)
                return false;

            carteiraOrigem.Saldo -= valor;
            carteiraDestino.Saldo += valor;

            var transacao = new Transaction
            {
                DeUserId = deUserId,
                ParaUserId = paraUserId,
                Valor = valor,
                Data = DateTime.UtcNow
            };

            _context.Transactions.Add(transacao);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AdicionarSaldoAsync(int userId, decimal valor)
        {
            if (valor <= 0) return false;

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null) return false;

            wallet.Saldo += valor;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Transaction>> ListarTransacoesAsync(int userId, DateTime? inicio = null, DateTime? fim = null)
        {
            var query = _context.Transactions
                .Where(t => t.DeUserId == userId || t.ParaUserId == userId);

            if (inicio.HasValue)
                query = query.Where(t => t.Data >= inicio.Value);

            if (fim.HasValue)
                query = query.Where(t => t.Data <= fim.Value);

            return await query.ToListAsync();

        }
    }
}
