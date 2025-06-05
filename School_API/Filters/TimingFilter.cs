using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace School_API.Filters
{
    public class TimingFilter : Attribute, IResourceFilter
    {
        private Stopwatch _stopwatch = new();

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _stopwatch.Start();
            Console.WriteLine("Inicio del recurso.");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _stopwatch.Stop();
            Console.WriteLine($"Tiempo total de ejecución: {_stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
