using SentinelFIM.Core.Hashing;
using System.Security.Cryptography;

namespace SentinelFIM.Infrastructure.Hashing
{
	public class SHA256HashProvider : IHashProvider
	{
		public string ComputeHash(Stream stream)
		{
			return Convert.ToHexString(SHA256.HashData(stream));
		}
	}
}
