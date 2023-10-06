using System.Data;
using System.Data.Common;
using NServiceBus.Extensibility;
using NServiceBus.Persistence.Sqlite.SagaPersister;
using NServiceBus.Sagas;

namespace NServiceBus.Persistence.Sqlite;
internal class SqliteSagaPersister : ISagaPersister
{
	private const string SAVE_COMMAND = "INSERT INTO SagaData(Id, Data, Metadata, PersistenceVersion, SagaTypeVersion, CorrelationId) VALUES (@Id, @Data, @Metadata, @PersistenceVersion, @SagaTypeVersion, @CorrelationId)";
	private const string COMPLETE_COMMAND = "DELETE FROM SagaData WHERE Id=@Id and Concurrency=@Concurrency";
	private const string GET_COMMAND = "SELECT Id, Metadata, Data, PersistenceVersion, SagaTypeVersion, CorrelationId, Concurrency FROM SagaData WHERE Id=@Id";
	private const string GET_BY_PROPERTY_COMMAND = "SELECT Id, Metadata, Data, PersistenceVersion, SagaTypeVersion, CorrelationId, Concurrency FROM SagaData WHERE CorrelationId=@LookupId";
	private const string UPDATE_COMMAND = "UPDATE SagaData SET Data= @Data, PersistenceVersion=@PersistenceVersion, SagaTypeVersion=@SagaTypeVersion,  Concurrency = @Concurrency + 1 WHERE Id=@Id and Concurrency=@Concurrency";
	private const string SAGA_CONTAINER_CONTEXT_KEY_PREFIX = "SagaDataContainer:";
	private const string SAGA_CONCURRENCY_CONTEXT_KEY_PREFIX = $"{SAGA_CONTAINER_CONTEXT_KEY_PREFIX}Concurrency:";

	public async Task Complete(IContainSagaData sagaData, ISynchronizedStorageSession session, ContextBag context, CancellationToken cancellationToken = default)
	{
		var sqliteSession = session.SqliteSession();

		if (sqliteSession?.Connection == null)
			return;

		var concurrency = context.Get<int>($"{SAGA_CONCURRENCY_CONTEXT_KEY_PREFIX}{sagaData.Id}");

		using var command = sqliteSession.Connection.CreateCommand();
		command.Transaction = sqliteSession.Transaction;
		command.CommandText = COMPLETE_COMMAND;

		command.AddParameter("Id", GetSagaId(sagaData));
		command.AddParameter("Concurrency", concurrency);

		var affected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

		if (affected != 1)
			throw new InvalidOperationException($"Optimistic concurrency violation when trying to complete saga {sagaData.GetType().FullName} {sagaData.Id}. Expected version {concurrency}.");
	}

	public async Task Save(IContainSagaData sagaData, SagaCorrelationProperty correlationProperty, ISynchronizedStorageSession session, ContextBag context, CancellationToken cancellationToken = default)
	{
		var sqliteSession = session.SqliteSession();

		if (sagaData == null)
			return;

		if (correlationProperty == null)
			return;

		if (sqliteSession?.Connection == null)
			return;

		var sagaDataType = sagaData.GetType();

		using var command = sqliteSession.Connection.CreateCommand();
		command.Transaction = sqliteSession.Transaction;
		command.CommandText = SAVE_COMMAND;
		command.AddParameter("Id", GetSagaId(sagaData));

		var metadata = new Dictionary<string, string>();
		if (sagaData.OriginalMessageId != null)
			metadata.Add("OriginalMessageId", sagaData.OriginalMessageId);

		if (sagaData.Originator != null)
			metadata.Add("Originator", sagaData.Originator);

		command.AddParameter("Metadata", Serializer.Serialize(metadata));
		command.AddParameter("Data", BuildSagaData(sagaData));
		command.AddParameter("PersistenceVersion", StaticVersions.PersistenceVersion);
		command.AddParameter("SagaTypeVersion", sagaDataType.Assembly.GetFileVersion());
		command.AddParameter("CorrelationId", SagaUniqueIdentity.FormatId(sagaDataType, correlationProperty.Name, correlationProperty.Value));

		await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
	}

	public async Task Update(IContainSagaData sagaData, ISynchronizedStorageSession session, ContextBag context, CancellationToken cancellationToken = default)
	{
		var sqliteSession = session.SqliteSession();

		if (sagaData == null)
			return;

		if (sqliteSession?.Connection == null)
			return;

		var concurrency = context.Get<int>($"{SAGA_CONCURRENCY_CONTEXT_KEY_PREFIX}{sagaData.Id}");

		var sagaDataType = sagaData.GetType();

		using var command = sqliteSession.Connection.CreateCommand();
		command.Transaction = sqliteSession.Transaction;
		command.CommandText = UPDATE_COMMAND;

		command.AddParameter("Id", GetSagaId(sagaData));
		command.AddParameter("PersistenceVersion", StaticVersions.PersistenceVersion);
		command.AddParameter("SagaTypeVersion", sagaDataType.Assembly.GetFileVersion());
		command.AddParameter("Data", BuildSagaData(sagaData));
		command.AddParameter("Concurrency", concurrency);

		var affected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
		if (affected != 1)
			throw new InvalidOperationException($"Optimistic concurrency violation when trying to save saga {sagaDataType.FullName} {sagaData.Id}. Expected version {concurrency}.");
	}

	public async Task<TSagaData> Get<TSagaData>(Guid sagaId, ISynchronizedStorageSession session, ContextBag context, CancellationToken cancellationToken = default) where TSagaData : class, IContainSagaData
	{
		return await GetByCommand<TSagaData>(session, GET_COMMAND, command =>
		{
			command.AddParameter("Id", GetSagaId(typeof(TSagaData), sagaId));
		}, context, cancellationToken).ConfigureAwait(false);
	}

	public async Task<TSagaData> Get<TSagaData>(string propertyName, object propertyValue, ISynchronizedStorageSession session, ContextBag context, CancellationToken cancellationToken = default) where TSagaData : class, IContainSagaData
	{
		var lookupId = SagaUniqueIdentity.FormatId(typeof(TSagaData), propertyName, propertyValue);

		return await GetByCommand<TSagaData>(session, GET_BY_PROPERTY_COMMAND, command =>
		{
			command.AddParameter("LookupId", lookupId);
		}, context, cancellationToken).ConfigureAwait(false);
	}

	private static async Task<TSagaData> GetByCommand<TSagaData>(ISynchronizedStorageSession session, string commandText, Action<DbCommand> setupCommand, ContextBag context, CancellationToken cancellationToken = default) where TSagaData : class, IContainSagaData
	{
		var sqliteSession = session.SqliteSession();

		if (sqliteSession?.Connection == null)
			return default;

		using var command = sqliteSession.Connection.CreateCommand();
		command.Transaction = sqliteSession.Transaction;
		command.CommandText = commandText;

		setupCommand(command);

		using var dataReader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);
		if (!await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
			return default;

		var id = await dataReader.GetGuidAsync(0, cancellationToken: cancellationToken).ConfigureAwait(false);
		//var sagaTypeVersionString = await dataReader.GetFieldValueAsync<string>(4, cancellationToken).ConfigureAwait(false);
		//var sagaTypeVersion = Version.Parse(sagaTypeVersionString);
		//var concurrency = await dataReader.GetFieldValueAsync<int>(2, cancellationToken).ConfigureAwait(false);
		ReadMetadata(dataReader.GetString(1), out var originator, out var originalMessageId);

		var sagaData = Serializer.Deserialize<TSagaData>(dataReader.GetString(2));
		sagaData.Id = id;
		sagaData.Originator = originator;
		sagaData.OriginalMessageId = originalMessageId;

		context.Set($"{SAGA_CONCURRENCY_CONTEXT_KEY_PREFIX}{sagaData.Id}", dataReader.GetInt32(6));

		return sagaData;
	}

	private static string GetSagaId(IContainSagaData sagaData) => GetSagaId(sagaData.GetType(), sagaData.Id);

	private static string GetSagaId(Type sagaDataType, Guid sagaId)
	{
		return sagaId.ToString();
	}

	private static string BuildSagaData(IContainSagaData sagaData)
	{
		var originalMessageId = sagaData.OriginalMessageId;
		var originator = sagaData.Originator;
		var id = sagaData.Id;
		sagaData.OriginalMessageId = null;
		sagaData.Originator = null;
		sagaData.Id = Guid.Empty;
		try
		{
			return Serializer.Serialize(sagaData);
		}
		finally
		{
			sagaData.OriginalMessageId = originalMessageId;
			sagaData.Originator = originator;
			sagaData.Id = id;
		}
	}

	static void ReadMetadata(string metaDataString, out string? originator, out string? originalMessageId)
	{
		var metadata = Serializer.Deserialize<Dictionary<string, string>>(metaDataString);
		metadata.TryGetValue("Originator", out originator);
		metadata.TryGetValue("OriginalMessageId", out originalMessageId);
	}
}
