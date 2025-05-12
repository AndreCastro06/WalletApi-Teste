using WalletApi.Data;
using WalletApi.Services;
using WalletApi.DTOs;
using Xunit;
using WalletApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

public class AuthServiceTests
{
    private IAuthService _authService;
    private AppDbContext _context;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new AppDbContext(options);

        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Key", "teste123456789teste123456789"},
            {"Jwt:Issuer", "WalletApi"},
            {"Jwt:Audience", "WalletApiUser"},
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(_context, config);
    }

    [Fact]
    public async Task Registrar_DeveCriarUsuario()
    {
        var dto = new RegisterDTO
        {
            Email = "teste@xunit.com",
            Nome = "Test",
            Senha = "123456"
        };

        var resultado = await _authService.RegistrarAsync(dto);
        Assert.True(resultado);

        var existe = await _authService.UsuarioExiste("teste@xunit.com");
        Assert.True(existe);
    }

    [Fact]
    public async Task Login_DeveRetornarTokenValido()
    {
        var dto = new RegisterDTO { Email = "x@x.com", Nome = "X", Senha = "123456" };
        await _authService.RegistrarAsync(dto);

        var token = await _authService.LoginAsync(new LoginDTO { Email = "x@x.com", Senha = "123456" });
        Assert.NotNull(token);
        Assert.Contains(".", token); // estrutura do JWT
    }
}