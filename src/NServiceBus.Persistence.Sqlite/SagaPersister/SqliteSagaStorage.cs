using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Features;
using NServiceBus.Sagas;

namespace NServiceBus.Persistence.Sqlite;

internal class SqliteSagaStorage : Feature
{
	internal SqliteSagaStorage()
	{
		Defaults(s => s.EnableFeatureByDefault<SqliteStorageSession>());

		DependsOn<NServiceBus.Features.Sagas>();
		DependsOn<SqliteStorageSession>();
	}

	protected override void Setup(FeatureConfigurationContext context) =>
		context.Services.AddSingleton<ISagaPersister, SqliteSagaPersister>();
}
