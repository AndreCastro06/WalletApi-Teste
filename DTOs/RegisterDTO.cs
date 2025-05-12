using System.ComponentModel.DataAnnotations;

namespace WalletApi.DTOs
{
    public class RegisterDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required, MinLength(6)]
        public string Senha { get; set; }
    }
}