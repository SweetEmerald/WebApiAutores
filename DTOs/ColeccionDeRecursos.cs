namespace WebApiAutores.DTOs
{
    public class ColeccionDeRecursos<T>: Recurso where T: Recurso  //Por ejemplo T puede ser AutorDTO y la clase AutorDTO debe heredar de Recurso
    {
        public List<T> Valores { get; set;  }
    }
}
