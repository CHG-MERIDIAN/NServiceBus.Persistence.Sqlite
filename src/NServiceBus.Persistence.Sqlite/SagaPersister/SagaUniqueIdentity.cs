using System.Security.Cryptography;
using System.Text;

namespace NServiceBus.Persistence.Sqlite.SagaPersister;

internal class SagaUniqueIdentity
{
	public static string FormatId(Type sagaType, string propertyName, object propertyValue)
	{
		if (propertyValue == null)
			throw new ArgumentNullException(nameof(propertyValue), $"Property {propertyName} is a correlation property on {sagaType.Name} but contains a null value. Make sure that all correlation properties on the SagaData have a defined value.");

		var inputBytes = Encoding.Default.GetBytes(propertyValue.ToString());
		var hashBytes = MD5.HashData(inputBytes);

		// generate a guid from the hash:
		var value = new Guid(hashBytes);

		var id = $"{sagaType.FullName.Replace('+', '-')}/{propertyName}/{value}";

		// raven has a size limit of 255 bytes == 127 unicode chars
		if (id.Length > 127)
		{
			// generate a guid from the hash:
			var hash = MD5.HashData(Encoding.Default.GetBytes(sagaType.FullName + propertyName));
			var key = new Guid(hash);

			id = $"MoreThan127/{key}/{value}";
		}

		return id;
	}
}
