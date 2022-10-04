using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    //public class Autor : IValidatableObject //Ponemos que herede de IValidatableObject para poner validaciones propias del modelo que estamos trabajando, es decir, una validación que combine distintas propiedades de la entidad Autor
    public class Autor
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage ="El campo {0} es requerido")] //Es para validar aquí también que el nombre no puede ser vacío o nulo
        [StringLength(maximumLength:120, ErrorMessage ="El campo {0} no debe tener más de {1} caracteres")] //Es una prueba para checar esta validación con place holders donde no podemos meter un nombre mayor de 5 caracteres
        [PrimeraLetraMayuscula] //Validación que creamos a partir de la clase PrimeraLetraMayuscula.cs que hereda de ValidationAttribute
        public string Nombre { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }


        /*[Range(18,120)] //Un ejemplo para poner un rango de edad
        [NotMapped] //Con este podremos tener propiedades en nuestros modelos que no se van a corresponder con una columna de la tabla (no está ligado este campo con la BD
        public int Edad { get; set; }
        
        [CreditCard] //Se va a encargar de validar la tarjeta de crédito, solo valida la numeración, que sea válida
        [NotMapped]
        public string TarjetaDeCredito { get; set; }
        
        [Url]
        [NotMapped]
        public string URL { get; set; }*/

        //public List<Libro> Libros { get; set; }

        /* public int Menor { get; set; }

         public int Mayor { get; set; }*/


        //A continuación las reglas de validación a nivel modelo
        //Al ejecutarse hay que recordar que primero se validan las reglas de validación a nivel de atributos (osea, las de aquí arriba) y después se revisan las validaciones a nivel moderno
        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) //Funcion para realizar las reglas de validación. Implementa la interfaz IValidatableObject
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraletra = Nombre[0].ToString();
                if (primeraletra != primeraletra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", //yield es para ir llenando el IEnumerable que debemos retornar, va a insertar ahí un elemento en esta colección 
                        new string[] { nameof(Nombre) }); //nameof nos permite obtener de forma rápida el nombre de una variable u operación. A nameof expression produces the name of a variable, type, or member as the string constant.
                }
            }*/

        /*if (Menor > Mayor)
        {
            yield return new ValidationResult("Este valor no puede ser mayor que el campo Mayor",
                new string[] { nameof(Menor) });
        }*/
        //}
    }
}
