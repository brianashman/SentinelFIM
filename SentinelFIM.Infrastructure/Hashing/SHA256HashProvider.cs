using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Identifiers;
using System.Security.Cryptography;

namespace SentinelFIM.Infrastructure.Hashing
{
	public class SHA256HashProvider : IHashProvider
	{
		public string Algorithm => AlgorithmType.SHA256;

		public string ComputeHash(Stream stream)
		{
			return Convert.ToHexString(SHA256.HashData(stream));
		}
	}
}
