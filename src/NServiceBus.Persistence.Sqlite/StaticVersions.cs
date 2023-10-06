using System.Diagnostics;
using System.Reflection;

namespace NServiceBus.Persistence.Sqlite;

static class StaticVersions
{
	public static readonly Version PersistenceVersion = GetFileVersion(Assembly.GetExecutingAssembly());

	internal static Version GetFileVersion(this Assembly assembly)
	{
		var version = FileVersionInfo.GetVersionInfo(assembly.Location);
		return new Version(
			major: version.FileMajorPart,
			minor: version.FileMinorPart,
			build: version.FileBuildPart,
			revision: version.FilePrivatePart);
	}
}