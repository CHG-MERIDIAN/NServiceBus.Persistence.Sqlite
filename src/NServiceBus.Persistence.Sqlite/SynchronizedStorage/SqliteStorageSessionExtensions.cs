using System.Data.Common;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sqlite;
using NServiceBus.Testing;

namespace NServiceBus;

/// <summary>
/// Extensions to manage Sqlite storage session.
/// </summary>
public static class SqliteStorageSessionExtensions
{
	/// <summary>
	/// Gets the current Sqlite storage session.
	/// </summary>
	/// <param name="session">The storage session.</param>
	public static ISqliteStorageSession? SqliteSession(this ISynchronizedStorageSession session) => session switch
	{
		SqliteSynchronizedStorageSession synchronizedStorageSession => synchronizedStorageSession.Session,
		TestableSqliteStorageSession testableStorageSession => testableStorageSession.Session,
		_ => throw new InvalidOperationException("It was not possible to retrieve a Sqlite storage session.")
	};

	/// <summary>
	/// Creates the required database schema
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	internal static async Task CreateSchema(this DbConnection connection, CancellationToken cancellationToken)
	{
		var command = connection.CreateCommand();

		command.CommandText =
		"""
			CREATE TABLE IF NOT EXISTS SagaData(
				Id string NOT NULL,
				Data string NOT NULL,
				Metadata string NOT NULL,
				PersistenceVersion string NOT NULL,
				SagaTypeVersion string NOT NULL,
				CorrelationId string NOT NULL,
				Concurrency int DEFAULT 1,
				PRIMARY KEY (id)
			);
			""";
		await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
	}
}
