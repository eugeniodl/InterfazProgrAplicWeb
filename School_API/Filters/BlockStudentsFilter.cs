using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace School_API.Filters
{
    public class BlockStudentsFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Simula un bloqueo por cualquier condición personalizada
            var forbidden = DateTime.Now.Second % 2 == 0;

            if (forbidden)
            {
                //context.Result = new ForbidResult();
                context.Result = new ObjectResult(new
                {
                    error = "Acceso denegado por política del filtro BlockStudentsFilter"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
