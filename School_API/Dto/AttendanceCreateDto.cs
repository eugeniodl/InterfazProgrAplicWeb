using System.ComponentModel.DataAnnotations;

namespace School_API.Dto
{
    public class AttendanceCreateDto
    {
        [Required(ErrorMessage = "El campo StudentId es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El StudentId debe ser mayor que cero.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Debe indicar si el estudiante asistió o no.")]
        public bool Present { get; set; }
    }
}

