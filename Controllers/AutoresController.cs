using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController] //Permitirá hacer validaciones automáticas respecto a la data recibida en nuestro controller
    [Route("api/autores")] //Tenemos que definir rutas para nuestros controladores, de esta manera, cuando hagamos una petición http hacia api/autores este controlador (AutoresController) va a manejar dicha petición
    public class AutoresController : ControllerBase //La clase debe heredar de ControllerBase
    {
        [HttpGet] //Creo una acción que va a responder a la petición HttpGet realizada hacia api/autores
        public ActionResult<List<Autor>> Get()
        {
            return new List<Autor>() {
                new Autor() { Id=1, Nombre="Felipe"},
                new Autor() { Id=2, Nombre="Claudia"}
            };
        }
    }
}
