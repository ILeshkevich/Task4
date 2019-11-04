using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GitApp.Hubs
{
    public class UploadStatusHub : Hub
    {
        public async Task Upload(string url)
        {
            await Clients.Caller.SendAsync("Upload", url);
        }
    }
}