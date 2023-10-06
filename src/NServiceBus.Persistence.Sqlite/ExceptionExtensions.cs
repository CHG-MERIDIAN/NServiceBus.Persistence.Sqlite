namespace NServiceBus.Persistence.Sqlite;

internal static class ExceptionExtensions
{
	public static bool IsCausedBy(this Exception ex, CancellationToken cancellationToken) => ex is OperationCanceledException && cancellationToken.IsCancellationRequested;
}