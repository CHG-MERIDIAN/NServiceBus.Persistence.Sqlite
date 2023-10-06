using System.Data.Common;

namespace NServiceBus.Persistence.Sqlite;

/// <summary>
/// Abstract the access to the internal storage session
/// </summary>
public interface ISqliteStorageSession
{
	DbTransaction? Transaction { get; }
	DbConnection? Connection { get; }
}
