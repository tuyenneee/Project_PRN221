using System.Security.Claims;
using System.Text.Json;

using WebRazor.Hubs;
using WebRazor.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WebRazor.Pages
{
    public class ChatModel : PageModel
    {
        private readonly PRN221DBContext db;
        private readonly IHubContext<MessageHub> hub;

        public ChatModel(PRN221DBContext _db, IHubContext<MessageHub> _hub)
        {
            db = _db;
            hub = _hub;
        }

        public List<Models.Account> users;
        public Models.Account user;
        public Models.Account toUser;
        public List<Chat> historyMessage = new List<Chat>();
        public List<Chat> historyMessageBetweenUser;
        public List<Models.Account> listAccountMess;
        public int accountID;
        public void OnGet(int? toUserId)
        {
            listAccountMess = db.Accounts.ToList();
            accountID= Int32.Parse(HttpContext.Session.GetString("ID"));
            //user = JsonSerializer.Deserialize<Models.Account>(User.FindFirstValue("Account"));
            user = db.Accounts.Where(x => x.AccountId == accountID).FirstOrDefault();
            int role = (int)user.Role;
            if(role ==2) {
				users = db.Accounts.Where(x => x.Role==1).ToList();
            }
            else
            {
				users = db.Accounts.Where(x => x.Role==2).ToList();
			}
			

            foreach (Models.Account u in users)
            {
                //Chat m = db.Chats.Where(x => x.FromUser == u.AccountId && x.ToUser == accountID).FirstOrDefault();
                Chat m = db.Chats.Where(x => (x.FromUser == u.AccountId && x.ToUser == accountID) || (x.ToUser == u.AccountId && x.FromUser == accountID)).OrderByDescending(x => x.Id).FirstOrDefault();
                if (m != null)
                {
                    //m.ToUserNavigation = u;
                    historyMessage.Add(m);
                }
            }


            if (toUserId != null)
            {
                toUser = db.Accounts.FirstOrDefault(x => x.AccountId == toUserId);
                historyMessageBetweenUser = db.Chats.Where(x =>
                (x.ToUser == toUserId && x.FromUser == user.AccountId) || (x.FromUser == toUserId && x.ToUser == user.AccountId))
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostSendMessage(string message, int idCurrentUser, int toUserId)
        {
            Chat mess = new Chat()
            {
                FromUser = idCurrentUser,
                ToUser = toUserId,
                Message = message,
                CreateDate = DateTime.Now,
				GroupId=1,
				AccountId = toUserId
            };
            db.Add(mess);
            db.SaveChanges();
            await hub.Clients.All.SendAsync("ReloadDocuments");

            return Redirect($"./Chat?toUserId={toUserId}");
        }
        
    }
}
