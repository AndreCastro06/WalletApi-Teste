using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletApi.DTOs;
using WalletApi.Services;


namespace WalletApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    private int ObterUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    [HttpGet("saldo")]
    public async Task<IActionResult> ConsultarSaldo()
    {
        var userId = ObterUserId();
        var saldo = await _walletService.ConsultarSaldoAsync(userId);
        return Ok(new { Saldo = saldo });
    }

    [HttpPost("deposito")]
    public async Task<IActionResult> AdicionarSaldo([FromBody] AdicionarSaldoDTO dto)
    {
        if (!ModelState.IsValid || dto.Valor <= 0)
            return BadRequest(new { Erro = "Valor inválido para depósito." });

        var userId = ObterUserId();
        var sucesso = await _walletService.AdicionarSaldoAsync(userId, dto.Valor);
        if (!sucesso) return BadRequest(new { Erro = "Erro ao adicionar saldo." });

        return Ok(new { Mensagem = "Saldo adicionado com sucesso." });
    }

    [HttpPost("transferencia")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaDTO dto)
    {
        if (!ModelState.IsValid || dto.Valor <= 0 || dto.ParaUserId == ObterUserId())
            return BadRequest(new { Erro = "Transferência inválida." });

        var deUserId = ObterUserId();
        var sucesso = await _walletService.TransferirAsync(deUserId, dto.ParaUserId, dto.Valor);
        if (!sucesso) return BadRequest(new { Erro = "Transferência não realizada. Verifique o saldo ou os dados." });

        return Ok(new { Mensagem = "Transferência realizada com sucesso." });
    }

    [HttpGet("transacoes")]
    public async Task<IActionResult> ListarTransacoes([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        var userId = ObterUserId();
        var transacoes = await _walletService.ListarTransacoesAsync(userId, inicio, fim);
        return Ok(transacoes);
    }
}