using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")] //Es para validar aquí también que el nombre no puede ser vacío o nulo
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres")] //Es una prueba para checar esta validación con place holders donde no podemos meter un nombre mayor de 5 caracteres
        [PrimeraLetraMayuscula] //Validación que creamos a partir de la clase PrimeraLetraMayuscula.cs que hereda de ValidationAttribute
        public string Nombre { get; set; }
    }
}
