using System.Text.Json;
using System.Text.Json.Serialization;

namespace NServiceBus.Persistence.Sqlite;

internal static class Serializer
{
	private static JsonSerializerOptions options => new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
	};

	public static T? Deserialize<T>(string data)
	{
		return JsonSerializer.Deserialize<T>(data, options);
	}

	public static string Serialize(object target)
	{
		try
		{
			return JsonSerializer.Serialize(target, options);
		}
		catch (Exception exception)
		{
			throw new SerializationException(exception);
		}
	}
}
