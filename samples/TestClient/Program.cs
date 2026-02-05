
using TestClient;

Console.Title = "Samples.SimpleSaga";
var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSaga");

var persistence = endpointConfiguration.UsePersistence<SqlitePersistence>();
persistence.UseConnectionString("Data Source=file:SagaStore.db");


endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.UseTransport<LearningTransport>();

var endpointInstance = await Endpoint.Start(endpointConfiguration)
	.ConfigureAwait(false);

Console.WriteLine();
Console.WriteLine("Storage locations:");
Console.WriteLine($"Learning Transport: {LearningLocationHelper.TransportDirectory}");

Console.WriteLine();
Console.WriteLine("Press 'Enter' to send a StartOrder message");
Console.WriteLine("Press any other key to exit");

while (true)
{
	Console.WriteLine();
	if (Console.ReadKey().Key != ConsoleKey.Enter)
	{
		break;
	}
	var orderId = Guid.NewGuid();
	var startOrder = new StartOrder
	{
		OrderId = orderId
	};
	await endpointInstance.SendLocal(startOrder)
		.ConfigureAwait(false);
	Console.WriteLine($"Sent StartOrder with OrderId {orderId}.");
}

await endpointInstance.Stop()
	.ConfigureAwait(false);
