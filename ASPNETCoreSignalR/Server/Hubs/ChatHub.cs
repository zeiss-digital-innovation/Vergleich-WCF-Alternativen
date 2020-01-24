using Microsoft.AspNetCore.SignalR;
using Server.Database;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private DataContext _context;
        public ChatHub(DataContext context) => _context = context;
        ~ChatHub() => _context.Dispose();

        public async Task SendMessage(string user, string message)
        {
            var newMessage = new Message(user, message, DateTime.Now);
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", newMessage);
        }
        
        public async Task Join(string name)
        {
            var newUser = new User(Context.ConnectionId, name);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("UserlistUpdated", _context.Users.ToList());
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("GetChats", _context.Messages.ToList());
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = _context.Users.Find(Context.ConnectionId);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("UserlistUpdated", _context.Users.ToList());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
