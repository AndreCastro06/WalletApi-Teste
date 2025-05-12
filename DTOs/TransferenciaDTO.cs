using System.ComponentModel.DataAnnotations;

namespace WalletApi.DTOs
{
    public class TransferenciaDTO
    {
        [Required(ErrorMessage = "O campo 'ParaUserId' � obrigat�rio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Id do destinat�rio inv�lido.")]
        public int ParaUserId { get; set; }

        [Required(ErrorMessage = "O campo 'Valor' � obrigat�rio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}