using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [PrimeraLetraMayuscula] //La usamos aquí también (aparte del nombre del autor en la clase Autor.cs) como ejemplo de una validación personalizada por atributo
        [StringLength(maximumLength:250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }

        //public int AutorId { get; set; }
        //public Autor Autor { get; set; }

    }
}
