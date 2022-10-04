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

namespace WebApiAutores.Controllers.V1
{
    [ApiController] //Permitirá hacer validaciones automáticas respecto a la data recibida en nuestro controller
    [Route("api/autores")]
    [CabeceraEstaPresenteAttribute("x-version","1")]
    //[Route("api/v1/autores")] //Tenemos que definir rutas para nuestros controladores, de esta manera, cuando hagamos una petición http hacia api/autores este controlador (AutoresController) va a manejar dicha petición
    //[Route("api/[controller]")] //También funciona poniendole [controller] que es como un place holder o una variable que en tiempo de ejecución se va a sustituir por el nombre del controlador (osea autores)
    //[Authorize]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase //La clase debe heredar de ControllerBase
    {
        //public WebApiAutores.ApplicationDbContext Context { get; }
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        /*private readonly IServicio servicio;
private readonly ServicioTransient servicioTransient;
private readonly ServicioScoped servicioScoped;
private readonly ServicioSingleton servicioSingleton;
private readonly ILogger<AutoresController> logger;*/

        /*public AutoresController(WebApiAutores.ApplicationDbContext context, 
            IServicio servicio, 
            ServicioTransient servicioTransient, 
            ServicioScoped servicioScoped, 
            ServicioSingleton servicioSingleton,
            ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }*/

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }

        /*[HttpGet("GUID")]
        //[ResponseCache(Duration =10)] //Este es un filtro que si llega una primera petición HTTP a esta ruta, no pasa nada, se retorna lo de aquí abajo, pero las próximas peticiones que lleguen en los 10 segundos se van a servir del cache, osea, la primer respuesta se va a guardar en memoria y eso es lo que se les va a responder a los demás usuarios en las peticiones de los próximos 10 segundos
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok(new { 
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),

                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.ObtenerScoped(),

                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.ObtenerSingleton(),
            }
                );
        }*/

        //[HttpGet] //Creo una acción que va a responder a la petición HttpGet realizada hacia api/autores
        //[HttpGet("listado")] //api/autores/listado
        //[HttpGet("/listado")] //la rura sustituye a "api/autores" por "/listado"
        //[ResponseCache(Duration = 10)]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        // public async Task<ActionResult<List<Autor>>> Get()
        //{
        /*throw new NotImplementedException(); //Excepción de ejemplo 
        logger.LogInformation("Estamos obteniendo los autores");
        logger.LogWarning("Este es un mensaje de prueba");*/
        //return await context.Autores.Include(x=> x.Libros).ToListAsync();
        //}

        /*[HttpGet("primero")] //Creo una acción que va a responder a la petición HttpGet realizada hacia api/autores/primero y va a regresar el primer autor
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }*/

        [HttpGet(Name = "obtenerAutoresv1")] //api/autores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(autor =>autor.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }


        //[HttpGet("configuraciones")]
        //public ActionResult<string> ObtenerConfiguracion()
        //{
        //    return configuration["apellido"];
        //}

        [HttpGet("{id:int}", Name = "obtenerAutorv1")]
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

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre(string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpPost(Name = "crearAutorv1")]
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

            return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorv1")] //api/autores/1 (o 2 o 3 o lo que sea)
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

        /// <summary>
        /// Borra un autor
        /// </summary>
        /// <param name="id">Id del autor a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "borrarAutorv1")] //api/autores/2 (delete hacia el autor con id 2)
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
