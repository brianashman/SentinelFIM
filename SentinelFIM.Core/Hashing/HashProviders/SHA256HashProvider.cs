using System.Security.Cryptography;

namespace SentinelFIM.Core.Hashing.HashProviders
{
	public class SHA256HashProvider : IHashProvider
	{
		public string ComputeHash(Stream stream)
		{
			return Convert.ToHexString(SHA256.HashData(stream));
		}
	}
}
