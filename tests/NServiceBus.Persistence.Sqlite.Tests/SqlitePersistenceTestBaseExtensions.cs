using NServiceBus.Sagas;

namespace NServiceBus.Persistence.Sqlite.Tests;

internal static class SqlitePersistenceTestBaseExtensions
{
	public static SagaCorrelationProperty CreateMetadata<T>(this SqlitePersistenceTestBase test, IContainSagaData sagaEntity)
	{
		_ = test;

		var metadata = SagaMetadata.Create(typeof(T));

		metadata.TryGetCorrelationProperty(out SagaMetadata.CorrelationPropertyMetadata correlationPropertyMetadata);

		var propertyInfo = metadata.SagaEntityType.GetProperty(correlationPropertyMetadata.Name);
		var value = propertyInfo.GetValue(sagaEntity);

		var correlationProperty = new SagaCorrelationProperty(correlationPropertyMetadata.Name, value);

		return correlationProperty;
	}
}
