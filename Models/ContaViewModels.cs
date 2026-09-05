using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.Models
{
   
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(120, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter ao menos 6 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a senha.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }

    // ViewModel usado na tela de Login
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;
    }
}
