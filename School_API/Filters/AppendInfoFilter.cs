using Microsoft.AspNetCore.Mvc.Filters;

namespace School_API.Filters
{
    public class AppendInfoFilter : Attribute, IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("Preparando respuesta para enviar...");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("Respuesta enviada.");
        }
    }
}

