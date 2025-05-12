using WalletApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace WalletApi.Data;

public static class SeedData
{
    public static void Inicializar(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.Migrate();

        if (context.Users.Any()) return; // Evita duplicação

        CriarUsuario(context, "ana@wallet.com", "Ana", "senha123", 1000);
        CriarUsuario(context, "bruno@wallet.com", "Bruno", "senha123", 500);
        CriarUsuario(context, "carla@wallet.com", "Carla", "senha123", 0);

        context.SaveChanges();

        var ana = context.Users.First(u => u.Email == "ana@wallet.com");
        var bruno = context.Users.First(u => u.Email == "bruno@wallet.com");

        context.Transactions.Add(new Transaction
        {
            DeUserId = ana.Id,
            ParaUserId = bruno.Id,
            Valor = 200,
            Data = DateTime.UtcNow.AddDays(-2)
        });

        ana.Wallet.Saldo -= 200;
        bruno.Wallet.Saldo += 200;

        context.SaveChanges();
    }

    private static void CriarUsuario(AppDbContext context, string email, string nome, string senha, decimal saldoInicial)
    {
        CriarHashSenha(senha, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Email = email.ToLower(),
            Nome = nome,
            PasswordHash = hash,
            PasswordSalt = salt,
            Wallet = new Wallet { Saldo = saldoInicial }
        };

        context.Users.Add(user);
    }

    private static void CriarHashSenha(string senha, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
    }
}