using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using SharedLibrary;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private DataContext _context;

        public UsersController(DataContext context) => _context = context;
        ~UsersController() => _context.Dispose();

        // GET: api/Users
        [HttpGet]
        public List<User> Get()
        {
            return _context.Users.ToList();
        }

        // POST: api/Users
        [HttpPost]
        public async Task Post([FromBody] string value)
        {
            var user = new User(value);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // DELETE: api/Users/name
        [HttpDelete("{name}")]
        public async Task Delete(string name)
        {
            var user = await _context.Users.Where(u => u.Name == name).ToArrayAsync();

            _context.Users.Remove(user[0]);
            await _context.SaveChangesAsync();
        }
    }
}
