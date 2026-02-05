using NServiceBus.Sagas;

namespace NServiceBus.Persistence.Sqlite.Tests;

internal static class SqlitePersistenceTestBaseExtensions
{
	public static SagaCorrelationProperty CreateMetadata<T>(this SqlitePersistenceTestBase test, IContainSagaData sagaEntity) where T : Saga
	{
		_ = test;

		var metadata = SagaMetadata.Create<T>();

		metadata.TryGetCorrelationProperty(out SagaMetadata.CorrelationPropertyMetadata correlationPropertyMetadata);

		var propertyInfo = metadata.SagaEntityType.GetProperty(correlationPropertyMetadata.Name);
		var value = propertyInfo.GetValue(sagaEntity);

		var correlationProperty = new SagaCorrelationProperty(correlationPropertyMetadata.Name, value);

		return correlationProperty;
	}
}
