using Microsoft.AspNetCore.Mvc;
using WalletApi.DTOs;
using WalletApi.Services;

namespace WalletApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterDTO dto)
    {
        var sucesso = await _authService.RegistrarAsync(dto);
        if (!sucesso)
            return BadRequest(new { Erro = "E-mail já está em uso." });

        return Ok(new { Mensagem = "Usuário registrado com sucesso." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { Erro = "Credenciais inválidas." });

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no login: {ex.Message}");
            return StatusCode(500, new { erro = ex.Message }); // Exibe a mensagem real do erro
        }
    }
}
