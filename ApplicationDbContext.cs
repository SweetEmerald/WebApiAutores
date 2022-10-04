using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    //public class ApplicationDbContext : DbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options) //Con el DbContextOptions es para pasar el connection string
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AutorLibro>().HasKey(al => new { al.AutorId, al.LibroId }); //Va a tener una llave compuesta por autorid y libro id de AutorLibro
        }

        //Configurar las tablas que vamos a generar a traves de los comandos de Entity Framework
        public DbSet<Autor> Autores { get; set; } //Aquí le estoy diciendo, creame una tabla a partir del esquema o de las propiedades de Autor
        public DbSet<Libro> Libros { get; set; } //Aquí le estoy diciendo, creame una tabla a partir del esquema o de las propiedades del Libros

        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<AutorLibro> AutoresLibros { get; set; }
    }
}
