using System.ComponentModel.DataAnnotations;

namespace SGC.Clinica.Api.Dtos
{
    public class AddPatientDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DateOfBirth { get; set; }
    }
}
