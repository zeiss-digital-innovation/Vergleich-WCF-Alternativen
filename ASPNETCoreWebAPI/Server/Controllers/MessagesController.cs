using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Database;
using SharedLibrary;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private DataContext _context;
        public MessagesController(DataContext context) => _context = context;
        ~MessagesController() => _context.Dispose();

        // GET: api/Messages
        [HttpGet]
        public List<Message> Get()
        {
            return _context.Messages.ToList();
        }

        // POST: api/Messages
        [HttpPost]
        public async Task Post([FromBody] string content)
        {
            string[] data = content.Split(',');
            var message = new Message(data[0], data[1], DateTime.Now);

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}
