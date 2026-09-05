using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaConsultasUVV.Models
{
    public class Consulta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(100, ErrorMessage = "A especialidade deve ter no máximo 100 caracteres.")]
        public string Especialidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data e hora da consulta são obrigatórias.")]
        [Display(Name = "Data/Hora")]
        [DataType(DataType.DateTime)]
        public DateTime DataHora { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }

    
        [Required]
        [Display(Name = "Usuário")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
    }
}
