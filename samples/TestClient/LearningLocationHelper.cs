using System.Reflection;

namespace TestClient;

public static class LearningLocationHelper
{
	public static string TransportDirectory { get; }
	public static string SagaDirectory { get; }

	static LearningLocationHelper()
	{
		var location = Assembly.GetExecutingAssembly().Location;
		var runningDirectory = Directory.GetParent(location).FullName;
		SagaDirectory = Path.Combine(runningDirectory, ".sagas");
		TransportDirectory = Path.GetFullPath(Path.Combine(runningDirectory, @"..\..\..\"));
	}

	public static string TransportDelayedDirectory(DateTime dateTime)
	{
		return Path.Combine(TransportDirectory, ".delayed", dateTime.ToString("yyyyMMddHHmmss"));
	}

	public static string GetSagaLocation<T>(Guid sagaId)
		where T : Saga
	{
		return Path.Combine(SagaDirectory, typeof(T).Name, $"{sagaId}.json");
	}
}
