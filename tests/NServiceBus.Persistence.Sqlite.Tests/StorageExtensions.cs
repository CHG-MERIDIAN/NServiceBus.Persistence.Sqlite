using System.Data.Common;

namespace NServiceBus.Persistence.Sqlite.Tests;
internal static class StorageExtensions
{
	public static async Task<bool> RecordExists(this DbConnection connection, string commandText)
	{
		var command = connection.CreateCommand();
		command.CommandText = commandText;

		using var reader = await command.ExecuteReaderAsync();
		return await reader.ReadAsync();
	}

	public static async Task<T> GetFirst<T>(this DbConnection connection, string commandText)
	{
		var command = connection.CreateCommand();
		command.CommandText = commandText;

		using var reader = await command.ExecuteReaderAsync();
		if (await reader.ReadAsync())
			return reader.GetFieldValue<T>(0);

		return default;
	}
}
