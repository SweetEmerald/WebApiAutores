using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace; //Me va a dar el namespace del controlador  Ejemplo: Controllers.V1
            var versionAPI = namespaceControlador.Split('.').Last().ToLower(); //v1
            controller.ApiExplorer.GroupName = versionAPI;
        }
    }
}
