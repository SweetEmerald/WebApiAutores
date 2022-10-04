using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V2
{
    [ApiController] //Permitirá hacer validaciones automáticas respecto a la data recibida en nuestro controller
    //[Route("api/v2/autores")] //Tenemos que definir rutas para nuestros controladores, de esta manera, cuando hagamos una petición http hacia api/autores este controlador (AutoresController) va a manejar dicha petición
    [Route("api/autores")]
    [CabeceraEstaPresenteAttribute("x-version", "2")]
    //[Route("api/[controller]")] //También funciona poniendole [controller] que es como un place holder o una variable que en tiempo de ejecución se va a sustituir por el nombre del controlador (osea autores)
    //[Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase //La clase debe heredar de ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }


        [HttpGet(Name = "obtenerAutoresv2")] //api/autores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutorv2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorBD => autorBD.AutoresLibros)
                .ThenInclude(autorlibroDB => autorlibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);
            if (autor == null)
                return NotFound();

            var dto = mapper.Map<AutorDTOConLibros>(autor);
            return dto;
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev2")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre(string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpPost(Name = "crearAutorv2")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre); //Checa en la tabla de autores si existe un autor con el nombre que le estamos pasando

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);//Lo estoy marcando como próximo a crear, pero no se ha creado todavía
            await context.SaveChangesAsync(); //Con esta instrucción guardamos los cambios

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutorv2", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorv2")] //api/autores/1 (o 2 o 3 o lo que sea)
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await context.Autores.AnyAsync(a => a.Id == id);

            if (!existe)
                return NotFound();

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor); //Marcando el autor que vamos a actualizar

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarAutorv2")] //api/autores/2 (delete hacia el autor con id 2)
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(a => a.Id == id);

            if (!existe)
                return NotFound();

            context.Remove(new Autor() { Id = id }); //No estoy creando un nuevo autor, estoy instanciando un objeto de tipo autor porque es lo que tengo que pasarle al EntityFrameworkCore para que pueda borrar el autor

            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
