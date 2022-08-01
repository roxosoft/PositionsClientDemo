using Grpc.Core;
using Grpc.Net.Client;
using System.Net.Security;
using TestPositionClient.Gateway;

var sslOptions = new SslClientAuthenticationOptions
{
    // Workaround to ingore certificate errors. The below line of code should not be
    // in the production system.
    RemoteCertificateValidationCallback = delegate { return true; },
};

var httpHandler = new SocketsHttpHandler
{

    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
    EnableMultipleHttp2Connections = true,
    SslOptions = sslOptions
};

var reusableChannel = GrpcChannel.ForAddress("https://localhost:7001",
    new GrpcChannelOptions { HttpHandler = httpHandler });

var positionsClient = new PositionsService.PositionsServiceClient(reusableChannel);

using var positionsSubscription = positionsClient.Subscribe(new PositionsSubscriptionRequestGrpcMessage());

try
{
    while (await positionsSubscription.ResponseStream.MoveNext(new CancellationToken(false)))
    {
        var positionsUpdateMessage = positionsSubscription.ResponseStream.Current;

        Console.WriteLine("Positions batch received");
        foreach (var calcDetail in positionsUpdateMessage.CalculationDetails)
        {
            Console.WriteLine($"{calcDetail.ProductCategoryAbbreviation} | {calcDetail.Product, -25} | {calcDetail.SourceProduct, -25} | {calcDetail.PortfolioId, -4} | {calcDetail.CalculationDate.ToDateTime(),-8:yyyy-MM} | {calcDetail.Amount, 10}");
        }
        Console.WriteLine();
    }
}
catch (RpcException)
{

}