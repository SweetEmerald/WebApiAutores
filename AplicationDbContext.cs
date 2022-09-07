using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class AplicationDbContext : DbContext
    {
        protected AplicationDbContext(DbContextOptions options): base(options) //Con el DbContextOptions es para pasar el connection string
        {

        }

        //Configurar las tablas que vamos a generar a traves de los comandos de Entity Framework
        public DbSet<Autor> Autores { get; set; } //Aquí le estoy diciendo, creame una tabla a partir del esquema o de las propiedades de Autor
    }
}
