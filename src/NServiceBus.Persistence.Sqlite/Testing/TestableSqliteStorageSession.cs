using NServiceBus.Persistence;
using NServiceBus.Persistence.Sqlite;

namespace NServiceBus.Testing;

/// <summary>
/// A fake implementation for <see cref="ISynchronizedStorageSession" /> for testing purposes.
/// </summary>
public class TestableSqliteStorageSession : ISynchronizedStorageSession
{
	/// <summary>
	/// The session which is retrieved by calling <see cref="SqliteStorageSessionExtensions.SqliteSession" />.
	/// </summary>
	public ISqliteStorageSession Session { get; }

	/// <summary>
	/// Creates a new instance of <see cref="TestableSqliteStorageSession" />
	/// using the provided <see cref="ISqliteStorageSession" />.
	/// </summary>
	public TestableSqliteStorageSession(ISqliteStorageSession session)
	{
		Session = session;
	}
}
