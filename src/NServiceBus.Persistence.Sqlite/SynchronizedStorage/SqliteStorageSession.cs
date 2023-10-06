using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Features;
using NServiceBus.Settings;

namespace NServiceBus.Persistence.Sqlite;

internal class SqliteStorageSession : Feature
{
	internal const string CONNECTION_STRING = "SqlliteStorageConnectionString";

	public SqliteStorageSession() => DependsOn<SynchronizedStorage>();

	protected override void Setup(FeatureConfigurationContext context)
	{
		context.Services.AddScoped<ICompletableSynchronizedStorageSession>(provider =>
		{
			var settings = provider.GetRequiredService<IReadOnlySettings>();
			var connectionString = settings.GetConnectionString();

			return new SqliteSynchronizedStorageSession(connectionString);
		});

		context.Services.AddScoped(sp => (sp.GetService<ICompletableSynchronizedStorageSession>() as SqliteSynchronizedStorageSession)?.Session!);

		context.RegisterStartupTask(provider =>
		{
			var settings = provider.GetRequiredService<IReadOnlySettings>();
			var connectionString = settings.GetConnectionString();

			return new CreateSchemaStartupTask(connectionString);
		});
	}
}
