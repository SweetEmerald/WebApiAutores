using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones
{
    public class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) //value=valor del campo Ejemplo: Felipe     validationContext me da acceso a varios valores, como el objeto Autores completo
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            { 
                //return base.IsValid(value, validationContext);
                return ValidationResult.Success;   //No quiero hacer nada si es nulo o vacío porque no quiero tener doble validación, ya que en Autor.cs está el [Required]
            }
            
            var primeraletra = value.ToString()[0].ToString();
            
            if (primeraletra != primeraletra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }
            
            return ValidationResult.Success;
        }
    }
}
