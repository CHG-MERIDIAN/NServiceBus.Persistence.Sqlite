using NServiceBus.Logging;

namespace TestClient;

public class OrderSaga : Saga<OrderSagaData>,
	IAmStartedByMessages<StartOrder>,
	IHandleMessages<CompleteOrder>,
	IHandleTimeouts<CancelOrder>
{
	readonly static ILog s_log = LogManager.GetLogger<OrderSaga>();

	protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
	{
		mapper.MapSaga(sagaData => sagaData.OrderId)
			.ToMessage<StartOrder>(message => message.OrderId)
			.ToMessage<CompleteOrder>(message => message.OrderId);
	}

	public async Task Handle(StartOrder message, IMessageHandlerContext context)
	{
		// Correlation property Data.OrderId is automatically assigned with the value from message.OrderId;
		s_log.Info($"StartOrder received with OrderId {message.OrderId}");

		s_log.Info($@"Sending a CompleteOrder that will be delayed by 10 seconds Stop the endpoint now to see the saga data in:{LearningLocationHelper.GetSagaLocation<OrderSaga>(Data.Id)}");
		var completeOrder = new CompleteOrder
		{
			OrderId = Data.OrderId
		};
		var sendOptions = new SendOptions();
		sendOptions.DelayDeliveryWith(TimeSpan.FromSeconds(10));
		sendOptions.RouteToThisEndpoint();
		await context.Send(completeOrder, sendOptions)
			.ConfigureAwait(false);

		var timeout = DateTime.UtcNow.AddSeconds(30);
		s_log.Info($@"Requesting a CancelOrder that will be executed in 30 seconds. Stop the endpoint now to see the timeout data in the delayed directory {LearningLocationHelper.TransportDelayedDirectory(timeout)}");
		await RequestTimeout<CancelOrder>(context, timeout)
			.ConfigureAwait(false);
	}

	public Task Handle(CompleteOrder message, IMessageHandlerContext context)
	{
		s_log.Info($"CompleteOrder received with OrderId {message.OrderId}");
		MarkAsComplete();
		return Task.CompletedTask;
	}

	public Task Timeout(CancelOrder state, IMessageHandlerContext context)
	{
		s_log.Info($"CompleteOrder not received soon enough OrderId {Data.OrderId}. Calling MarkAsComplete");
		MarkAsComplete();
		return Task.CompletedTask;
	}
}
