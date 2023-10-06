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
}
