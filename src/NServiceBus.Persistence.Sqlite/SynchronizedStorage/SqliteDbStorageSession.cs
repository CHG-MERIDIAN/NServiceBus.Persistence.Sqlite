using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace NServiceBus.Persistence.Sqlite;

internal class SqliteDbStorageSession : ISqliteStorageSession, IDisposable
{
	public DbTransaction Transaction { get; private set; }
	public DbConnection Connection { get; private set; }

	public SqliteDbStorageSession(SqliteConnection connection, DbTransaction transaction)
	{
		Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
	}

	public void Dispose()
	{
		Transaction?.Dispose();
		Connection?.Dispose();
	}
}
