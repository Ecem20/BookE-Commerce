using Microsoft.AspNetCore.SignalR;
using MVCWeb.Models;
using MVCWeb.Service.IService;
using System.IdentityModel.Tokens.Jwt;

namespace MVCWeb.Web.Hubs
{
    public class SessionHub : Hub
    {
      
        public async Task ConnectionCheck(string Token, string browser)
        {
            try
            {
                if (Token != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(Token);

                    Information userInfo = new()
                    {
                        Browser = browser,
                        UserId = jwt.Payload.Sub
                    };

                    ConnectedUsers.Id.Add(Context.ConnectionId, userInfo);

                    foreach (var i in ConnectedUsers.Id.Values)
                    {
                        if (browser != i.Browser)
                        {
                            Close(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConnectionCheck method: {ex.Message}");
            }
        }

        private async Task Close(Information userInfo)
        {
            var usersToRemove = ConnectedUsers.Id.Where(c => c.Value == userInfo).ToList();
            foreach (var userToRemove in usersToRemove)
            {
                var userId = userToRemove.Key;
                var userConnection = userToRemove.Value;
                try
                {
                    await Clients.Client(userId).SendAsync("SignOut", true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending 'SignOut' message to user {userId}: {ex.Message}");
                }
                // Kullanıcıyı ConnectedUsers'tan kaldır
                ConnectedUsers.Id.Remove(userId);
            }
        }
    }
}