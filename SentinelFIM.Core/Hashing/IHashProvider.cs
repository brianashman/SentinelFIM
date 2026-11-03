using SentinelFIM.Core.Identifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Hashing
{
	public interface IHashProvider
	{
		string Algorithm { get; }
		string ComputeHash(Stream stream);
	}
}
