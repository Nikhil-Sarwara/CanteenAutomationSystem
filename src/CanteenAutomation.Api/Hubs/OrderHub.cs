using Microsoft.AspNetCore.SignalR;

public class OrderHub : Hub
{
    // This hub is used to push messages from the server.
    // Client-side methods like "ReceiveOrder" will be called from the controller.
}