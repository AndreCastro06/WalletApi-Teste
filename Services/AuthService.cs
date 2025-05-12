using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WalletApi.Data;
using WalletApi.DTOs;
using WalletApi.Models;

namespace WalletApi.Services;

public interface IAuthService
{
    Task<bool> UsuarioExiste(string email);
    Task<bool> RegistrarAsync(RegisterDTO dto);
    Task<string?> LoginAsync(LoginDTO dto);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<bool> UsuarioExiste(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email.ToLower());
    }

    public async Task<bool> RegistrarAsync(RegisterDTO dto)
    {
        if (await UsuarioExiste(dto.Email)) return false;

        CriarHashSenha(dto.Senha, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Email = dto.Email.ToLower(),
            Nome = dto.Nome,
            PasswordHash = hash,
            PasswordSalt = salt,
            Wallet = new Wallet { Saldo = 0 }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    //public async Task<string?> LoginAsync(LoginDTO dto)
    //{
    //    var user = await _context.Users.Include(u => u.Wallet)
    //        .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

    //    if (user == null || !VerificarSenha(dto.Senha, user.PasswordHash, user.PasswordSalt))
    //        return null;

    //    return GerarToken(user);
    //}

    public async Task<string?> LoginAsync(LoginDTO dto)
    {
        var user = await _context.Users.Include(u => u.Wallet)
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        if (user == null)
        {
            Console.WriteLine("Usuário não encontrado");
            return null;
        }

        var senhaOk = VerificarSenha(dto.Senha, user.PasswordHash, user.PasswordSalt);
        Console.WriteLine("Senha válida? " + senhaOk);

        if (!senhaOk)
            return null;

        return GerarToken(user);
    }



    private void CriarHashSenha(string senha, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
    }

    private bool VerificarSenha(string senha, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var hashComputado = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return hash.SequenceEqual(hashComputado);
    }

    private string GerarToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nome)
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}