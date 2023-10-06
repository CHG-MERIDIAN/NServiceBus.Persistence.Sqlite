using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Persistence.Sqlite;
using NServiceBus.Settings;

namespace NServiceBus;

/// <summary>
/// Provides configurations methods for the Sqlite storage
/// </summary>
public static class SqlitePersistenceExtensions
{
	/// <summary>
	/// Configures the storage to use the given connection string
	/// </summary>
	/// <param name="persistence">The persistence configuration object</param>
	/// <param name="connectionString">Connection string to the Sqlite database</param>
	public static PersistenceExtensions<SqlitePersistence> UseConnectionString(this PersistenceExtensions<SqlitePersistence> persistence, string connectionString)
	{
		persistence.GetSettings().Set(SqliteStorageSession.CONNECTION_STRING, connectionString);
		return persistence;
	}

	/// <summary>
	/// Gets the connectionstring setting
	/// </summary>
	/// <param name="settings">The settings container</param>
	/// <returns></returns>
	internal static string GetConnectionString(this IReadOnlySettings settings)
	{
		if (settings.TryGet(SqliteStorageSession.CONNECTION_STRING, out string value))
			return value;

		throw new InvalidOperationException($"Couldn't find connection string for Sqllite persistence. The connection to the database must be specified using the `{nameof(UseConnectionString)}` method.");
	}
}
