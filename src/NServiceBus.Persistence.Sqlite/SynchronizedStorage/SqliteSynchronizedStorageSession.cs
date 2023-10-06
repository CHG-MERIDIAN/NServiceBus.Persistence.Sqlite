using System.Transactions;
using Microsoft.Data.Sqlite;
using NServiceBus.Extensibility;
using NServiceBus.Outbox;
using NServiceBus.Transport;

namespace NServiceBus.Persistence.Sqlite;

internal sealed class SqliteSynchronizedStorageSession : ICompletableSynchronizedStorageSession
{
	public SqliteDbStorageSession? Session { get; private set; }

	private bool _ownsTransaction;
	private readonly string _connectionString;

	public SqliteSynchronizedStorageSession(string connectionString)
	{
		_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
	}

	public void Dispose()
	{
		if (_ownsTransaction)
		{
			Session?.Dispose();
		}
	}

	public ValueTask<bool> TryOpen(IOutboxTransaction transaction, ContextBag context, CancellationToken cancellationToken = new CancellationToken())
	{
		//if (transaction is NonDurableOutboxTransaction inMemOutboxTransaction)
		//{
		//	Transaction = inMemOutboxTransaction.Transaction;
		//	return new ValueTask<bool>(true);
		//}
		return new ValueTask<bool>(false);
	}

	public ValueTask<bool> TryOpen(TransportTransaction transportTransaction, ContextBag context, CancellationToken cancellationToken = new CancellationToken())
	{
		if (transportTransaction.TryGet(out Transaction ambientTransaction))
		{
			//Transaction = new NonDurableTransaction();
			//ownsTransaction = true;
			//ambientTransaction.EnlistVolatile(new EnlistmentNotification2(Transaction), EnlistmentOptions.None);
			return new ValueTask<bool>(true);
		}
		return new ValueTask<bool>(false);
	}

	public async Task Open(ContextBag context, CancellationToken cancellationToken = default)
	{
		var connection = new SqliteConnection(_connectionString);
		await connection.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
		var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
		Session = new SqliteDbStorageSession(connection, transaction);
		_ownsTransaction = true;
	}

	public async Task CompleteAsync(CancellationToken cancellationToken = default)
	{
		if (_ownsTransaction && Session != null)
		{
			await Session.Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
			await Session.Transaction.DisposeAsync().ConfigureAwait(false);

			Session.Connection.Dispose();
		}
	}
}
