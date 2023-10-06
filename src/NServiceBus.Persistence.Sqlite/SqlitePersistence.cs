using NServiceBus.Features;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sqlite;

namespace NServiceBus;

/// <summary>
/// Used to enable Sqlite persistence.
/// </summary>
public class SqlitePersistence : PersistenceDefinition
{
	internal SqlitePersistence()
	{
		Supports<StorageType.Sagas>(s => s.EnableFeatureByDefault<SqliteSagaStorage>());
		//Supports<StorageType.Subscriptions>(s => s.EnableFeatureByDefault<SqliteSubscriptionPersistence>());
		//Supports<StorageType.Outbox>(s => s.EnableFeatureByDefault<SqliteOutboxPersistence>());
	}
}
