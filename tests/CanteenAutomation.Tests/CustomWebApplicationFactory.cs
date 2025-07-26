// In: tests/CanteenAutomation.Tests/CustomWebApplicationFactory.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // --- Mock SignalR ---
            var hubContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IHubContext<OrderHub>));
            if (hubContextDescriptor != null)
            {
                services.Remove(hubContextDescriptor);
            }
            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            var mockHubContext = new Mock<IHubContext<OrderHub>>();
            mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
            services.AddSingleton(mockHubContext.Object);

            // --- Mock Redis ---
            var redisDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
            }
            var mockRedis = new Mock<IConnectionMultiplexer>();
            mockRedis.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                     .Returns(Mock.Of<IDatabase>());
            services.AddSingleton(mockRedis.Object);
        });
    }
}