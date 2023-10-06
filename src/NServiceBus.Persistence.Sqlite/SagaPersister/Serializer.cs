using System.Text.Json;
using System.Text.Json.Serialization;

namespace NServiceBus.Persistence.Sqlite;

internal static class Serializer
{
	private static JsonSerializerOptions s_options => new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
	};

	public static T? Deserialize<T>(string data)
	{
		return JsonSerializer.Deserialize<T>(data, s_options);
	}

	public static string Serialize(object target)
	{
		try
		{
			return JsonSerializer.Serialize(target, s_options);
		}
		catch (Exception exception)
		{
			throw new SerializationException(exception);
		}
	}
}
