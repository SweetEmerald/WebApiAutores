namespace WebApiAutores.Middlewares
{
    public static class LoguearRespuestaMiddlewareExtentions 
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaMiddleware>();
        }
    }

    public class LoguearRespuestaMiddleware
    {
        private RequestDelegate siguiente { get; }
        public LoguearRespuestaMiddleware(RequestDelegate siguiente)
        {
            this.siguiente = siguiente; //Lo inicicializamos como un campo
        }

        //Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext contexto, ILogger<LoguearRespuestaMiddleware> logger)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;
                await siguiente(contexto); //Con esta linea le permito continuar a la tubería de procesos

                //Las siguientes lineas se van a ejecutar cuando los demás middleware me estén devolviendo una respuesta
                ms.Seek(0, SeekOrigin.Begin);//Voy a ir al inicio del memoryStream
                string respuesta = new StreamReader(ms).ReadToEnd(); //Va a leer y guardar la respuesta que se le está devolviendo al cliente
                ms.Seek(0, SeekOrigin.Begin); //De nuevo volvemos a colocar el stream en la posición incial, así se le puede enviar la respuesta correcta al cliente
                await ms.CopyToAsync(cuerpoOriginalRespuesta); //Copiamos al cuerpo original
                contexto.Response.Body = cuerpoOriginalRespuesta; //Basicamente toda la manipulación hecha nos permite leer el stream, lo volvemos a colocar como estaba para que el usuario o el cliente final pueda utilizarlo

                //Ahora necesito obtener una instancia del ILogger 
                logger.LogInformation(respuesta);
            }
        }
    }
}
