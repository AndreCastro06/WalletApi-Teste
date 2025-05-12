using System.ComponentModel.DataAnnotations;

namespace WalletApi.DTOs
{
	public class AdicionarSaldoDTO
	{
		[Required(ErrorMessage = "O campo 'Valor' é obrigatório.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
		public decimal Valor { get; set; }
	}
}