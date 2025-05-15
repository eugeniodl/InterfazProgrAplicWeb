using EjemploAPI01.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EjemploAPI01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static List<User> Users = new List<User>
        {
            new User { Id = 1, Name = "Juan", Email = "juan@ejemplo.com" },
            new User { Id = 2, Name = "Pedro", Email = "pedro@ejemplo.com" },
            new User { Id = 3, Name = "Rosa", Email = "rosa@ejemplo.com" }
        };

        // Get: api/users
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Users;
        }
    }
}
