using System.Data.Common;

namespace NServiceBus.Persistence.Sqlite;

internal static class Extensions
{
	public static void AddParameter(this DbCommand command, string name, object value)
	{
		var parameter = command.CreateParameter();
		parameter.ParameterName = name;
		parameter.Value = value;
		command.Parameters.Add(parameter);
	}

	public static void AddParameter(this DbCommand command, string name, Version value)
	{
		command.AddParameter(name, value.ToString(4));
	}

	public static async Task<Guid> GetGuidAsync(this DbDataReader reader, int position, CancellationToken cancellationToken = default)
	{
		var type = reader.GetFieldType(position);

		if (type == typeof(string))
		{
			return new Guid(await reader.GetFieldValueAsync<string>(position, cancellationToken).ConfigureAwait(false));
		}
		return await reader.GetFieldValueAsync<Guid>(position, cancellationToken).ConfigureAwait(false);
	}

	public static async Task<DbConnection> OpenConnectionAsync(this DbConnection connection, CancellationToken cancellationToken)
	{
		try
		{
			await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
			return connection;
		}
		catch (Exception ex) when (ex.IsCausedBy(cancellationToken))
		{
			// copy the general catch but don't let another exception mask the OCE
			try
			{
				connection?.Dispose();
			}
			catch
			{
				// ignore error
			}

			throw;
		}
		catch (Exception)
		{
			connection?.Dispose();
			throw;
		}
	}
}
