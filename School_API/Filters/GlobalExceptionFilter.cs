using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace School_API.Filters
{
    public class GlobalExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine($"Excepción: {context.Exception.Message}");

            context.Result = new ObjectResult(new
            {
                Error = "Error inesperado",
                Details = context.Exception.Message
            })
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
