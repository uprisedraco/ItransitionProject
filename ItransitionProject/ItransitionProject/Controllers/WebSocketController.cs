using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace ItransitionProject.Controllers
{
    public class WebSocketController : Controller
    {
        public static Dictionary<int, List<WebSocket>> Subscribers = new Dictionary<int, List<WebSocket>>();

        [HttpGet("/ws")]
        public async Task Get(int itemId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                if (Subscribers.ContainsKey(itemId))
                {
                    Subscribers[itemId].Add(webSocket);
                }
                else
                {
                    Subscribers.Add(itemId, new List<WebSocket>() { webSocket });
                }
                var buffer = new byte[1024 * 4];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                Subscribers[itemId].Remove(webSocket);
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}
