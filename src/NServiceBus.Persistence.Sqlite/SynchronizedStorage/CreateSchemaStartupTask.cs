using Microsoft.Data.Sqlite;
using NServiceBus.Features;

namespace NServiceBus.Persistence.Sqlite;

internal class CreateSchemaStartupTask : FeatureStartupTask
{
	private readonly string _connectionString;

	public CreateSchemaStartupTask(string connectionString)
	{
		_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
	}

	protected override async Task OnStart(IMessageSession session, CancellationToken cancellationToken = default)
	{
		var connection = new SqliteConnection(_connectionString);
		await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
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

	protected override Task OnStop(IMessageSession session, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}
}
