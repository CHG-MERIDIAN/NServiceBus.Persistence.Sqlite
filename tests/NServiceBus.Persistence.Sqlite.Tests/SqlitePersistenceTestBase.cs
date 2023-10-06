using Microsoft.Data.Sqlite;
using NServiceBus.Testing;

namespace NServiceBus.Persistence.Sqlite.Tests;

public abstract class SqlitePersistenceTestBase
{
	protected TestableSqliteStorageSession SynchronizedSession { get; set; }

	[SetUp]
	public virtual async Task SetUp()
	{
		var connection = new SqliteConnection("Data Source=file:DataStore?mode=memory&cache=shared");
		await connection.OpenConnectionAsync(CancellationToken.None).ConfigureAwait(false);

		await connection.CreateSchema(CancellationToken.None).ConfigureAwait(false);

		var transaction = await connection.BeginTransactionAsync(CancellationToken.None).ConfigureAwait(false);
		var storageSession = new SqliteDbStorageSession(connection, transaction);

		SynchronizedSession = new TestableSqliteStorageSession(storageSession);
	}

	[TearDown]
	public virtual void TearDown()
	{
		SynchronizedSession.Session.Transaction.Dispose();
		SynchronizedSession.Session.Connection.Dispose();
	}
}
