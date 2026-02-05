using NServiceBus.Extensibility;
using Shouldly;

namespace NServiceBus.Persistence.Sqlite.Tests.SagaPersister;

internal class SqliteSagaPersisterTests : SqlitePersistenceTestBase
{
	public class CompleteMethod : SqliteSagaPersisterTests
	{
		[Test]
		public async Task Removes_Saga_When_Found()
		{
			var persister = new SqliteSagaPersister();

			var sagaId = Guid.NewGuid();

			var sagaEntity = new SagaData
			{
				Id = sagaId,
				SomeId = Guid.NewGuid()
			};

			var context = new ContextBag();

			await persister.Save(sagaEntity, this.CreateMetadata<SomeSaga>(sagaEntity), SynchronizedSession, context);

			(await SynchronizedSession.Session.Connection.RecordExists($"SELECT * FROM SagaData WHERE Id='{sagaId.ToString()}'")).ShouldBeTrue();

			var saga = await persister.Get<SagaData>(sagaId, SynchronizedSession, context);
			await persister.Complete(saga, SynchronizedSession, context);

			(await SynchronizedSession.Session.Connection.RecordExists($"SELECT * FROM SagaData WHERE Id='{sagaId.ToString()}'")).ShouldBeFalse();
		}
	}

	public class SaveMethod : SqliteSagaPersisterTests
	{
		[Test]
		public async Task Creates_New_Saga()
		{
			var persister = new SqliteSagaPersister();

			var sagaId = Guid.NewGuid();

			var sagaEntity = new SagaData
			{
				Id = sagaId,
				SomeId = Guid.NewGuid()
			};

			var context = new ContextBag();

			await persister.Save(sagaEntity, this.CreateMetadata<SomeSaga>(sagaEntity), SynchronizedSession, context);

			(await SynchronizedSession.Session.Connection.RecordExists($"SELECT * FROM SagaData WHERE Id='{sagaId.ToString()}'")).ShouldBeTrue();
		}
	}

	public class UpdateMethod : SqliteSagaPersisterTests
	{
		[Test]
		public async Task Updates_Existing_Saga()
		{
			var persister = new SqliteSagaPersister();

			var sagaId = Guid.NewGuid();

			var sagaEntity = new SagaData
			{
				Id = sagaId,
				SomeId = Guid.NewGuid()
			};

			var context = new ContextBag();

			await persister.Save(sagaEntity, this.CreateMetadata<SomeSaga>(sagaEntity), SynchronizedSession, context);

			var saga = await persister.Get<SagaData>(sagaId, SynchronizedSession, context);
			await persister.Update(saga, SynchronizedSession, context);

			(await SynchronizedSession.Session.Connection.GetFirst<int>($"SELECT Concurrency FROM SagaData WHERE Id='{sagaId.ToString()}'")).ShouldBe(2);
		}
	}

	public class GetMethod : SqliteSagaPersisterTests
	{
		[Test]
		public async Task Gets_Existing_Saga_By_Id()
		{
			var persister = new SqliteSagaPersister();

			var sagaId = Guid.NewGuid();

			var sagaEntity = new SagaData
			{
				Id = sagaId,
				SomeId = Guid.NewGuid()
			};

			var context = new ContextBag();

			await persister.Save(sagaEntity, this.CreateMetadata<SomeSaga>(sagaEntity), SynchronizedSession, context);

			var saga = await persister.Get<SagaData>(sagaId, SynchronizedSession, context);
			saga.ShouldNotBeNull();
		}

		[Test]
		public async Task Gets_Existing_Saga_By_CorrelationId()
		{
			var persister = new SqliteSagaPersister();

			var sagaId = Guid.NewGuid();

			var sagaEntity = new SagaData
			{
				Id = sagaId,
				SomeId = Guid.NewGuid()
			};

			var context = new ContextBag();

			await persister.Save(sagaEntity, this.CreateMetadata<SomeSaga>(sagaEntity), SynchronizedSession, context);

			var saga = await persister.Get<SagaData>("SomeId", sagaEntity.SomeId, SynchronizedSession, context);
			saga.ShouldNotBeNull();
		}
	}

	class SomeSaga : Saga<SagaData>, IAmStartedByMessages<StartMessage>
	{
		public Task Handle(StartMessage message, IMessageHandlerContext context)
		{
			return Task.CompletedTask;
		}

		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
		{
			mapper.MapSaga(saga => saga.SomeId).ToMessage<StartMessage>(m => m.SomeId);
		}
	}

	class StartMessage : IMessage
	{
		public Guid SomeId { get; set; }
	}

	class SagaData : ContainSagaData
	{
		public Guid SomeId { get; set; }
	}
}
