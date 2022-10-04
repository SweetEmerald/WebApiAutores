using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;
//using WebApiAutores.Servicios;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
            }).AddJsonOptions(x=>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
            
            
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            /*services.AddTransient<IServicio, ServicioA>(); //Ejemplo para ver que se puede llamar a ServicioA donde se instancia a IServicio

            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            services.AddTransient<MiFiltroDeAccion>(); //Implementamos el filtro que hicimos en una clase. Aquí estamos registrando ese servicio de filtro

            services.AddHostedService<EscribirEnArchivo>();*/

            //services.AddResponseCaching(); //Con esto y con el UseResponseCaching que pusimos en la función Configure ya podemos usar caché en nuestra aplicación

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                    ClockSkew = TimeSpan.Zero
                }) ; //Esto es para utilizar un sistema de Autenticación en nuestra aplicación 

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c  =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "WebAPIAutores", 
                    Version = "v1",
                    Description = "Este es un WebApi para trabajar con autores y libros",
                    Contact = new OpenApiContact
                    { 
                        Email="dulce@hotmail.com",
                        Name="Dulce Arcadio",
                        Url = new Uri("https://gavilan.blog")
                    },
                    License = new OpenApiLicense 
                    { 
                        Name= "MIT"
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebAPIAutores", Version = "v2" });
                c.OperationFilter<AgregarParametroHATEOAS>();
                c.OperationFilter<AgregarParametroXVersion>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
                { 
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                     }
                });

                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                c.IncludeXmlComments(rutaXML);
             });
            
            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("EsAdmin")); //podemos tener tantas políticas como queramos, por ejemplo, podemos crear otra para vendedor
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();

            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://www.apirequest.io").AllowAnyMethod().AllowAnyHeader().WithExposedHeaders(new string[] { "cantidadTotalRegistros"});
                });
            });

            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //Todo lo que está aquí son middleware, se ejecutan uno tras otro, como en fila, cuando uno termina, sigue el otro y al final el último le regresa al penultimo y este al anterior hasta llegar al primero

            //app.UseMiddleware<LoguearRespuestaMiddleware>();
            app.UseLoguearRespuestaHTTP(); //Este es un ejemplo de como podemos crear un middleware en una clase y luego mandarlo llamar desde aquí

            /*app.Map("/ruta1", app => //Introducimos otra posible rama de nuestra tubería de procesos donde si el usuario hace una petición a /ruta1 entonces se ejecuta este middleware
            { 
                app.Run(async contexto => //Con Run interceptas la tubería de procesos
                { 
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería"); 
                }); 
            });*/


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPIAutores v2");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            //app.UseResponseCaching(); //Con esto y con el AddResponseChaching que pusimos en la función ConfigureServices ya podemos usar caché en nuestra aplicación

            app.UseAuthorization();

            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapControllers();
            });
        }
    }
}
