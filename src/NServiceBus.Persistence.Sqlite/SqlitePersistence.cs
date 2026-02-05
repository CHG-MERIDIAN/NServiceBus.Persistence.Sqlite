using NServiceBus.Persistence;
using NServiceBus.Persistence.Sqlite;

namespace NServiceBus;

/// <summary>
/// Used to enable Sqlite persistence.
/// </summary>
public class SqlitePersistence : PersistenceDefinition, IPersistenceDefinitionFactory<SqlitePersistence>
{
	internal SqlitePersistence()
	{
		Supports<StorageType.Sagas, SqliteSagaStorage>();
	}

	public static SqlitePersistence Create()
	{
		return new SqlitePersistence();
	}
}
