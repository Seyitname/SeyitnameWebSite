using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SeyitnameWebSite.Data;
using System.Threading.Tasks;

namespace SeyitnameWebSite.Hubs;

// Gerçek zamanlı chat hub'ı - kullanıcıların sohbet edebilmesi için
public class ChatHub : Hub
{
    private readonly UserManager<User> _userManager;

    public ChatHub(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // Kullanıcı bağlandığında
    public override async Task OnConnectedAsync()
    {
        var user = await _userManager.GetUserAsync(Context.User);
        if (user != null && user.IsBanned)
        {
            // Banlı kullanıcıyı bağlantıyı kes
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();
    }

    // Mesaj gönderme metodu
    public async Task SendMessage(string message)
    {
        var user = await _userManager.GetUserAsync(Context.User);
        if (user == null || user.IsMuted || user.IsBanned)
            return;

        // Tüm bağlı kullanıcılara mesaj gönder
        await Clients.All.SendAsync("ReceiveMessage", user.UserName + user.Tag, message);
    }

    // Özel mesaj gönderme (opsiyonel)
    public async Task SendPrivateMessage(string receiverUserName, string message)
    {
        var sender = await _userManager.GetUserAsync(Context.User);
        if (sender == null || sender.IsMuted || sender.IsBanned)
            return;

        var receiver = await _userManager.FindByNameAsync(receiverUserName);
        if (receiver == null || receiver.IsBanned)
            return;

        // Gönderene ve alıcıya mesaj gönder
        await Clients.User(receiver.Id).SendAsync("ReceivePrivateMessage", sender.UserName + sender.Tag, message);
        await Clients.Caller.SendAsync("ReceivePrivateMessage", "Sen", message);
    }
}