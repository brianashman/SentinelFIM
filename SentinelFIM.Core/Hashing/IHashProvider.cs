using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Hashing
{
	public interface IHashProvider
	{
		string ComputeHash(Stream stream);
	}
}
