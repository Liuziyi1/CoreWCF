using Contract;
using CoreWCF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
	[ServiceBehavior]
	public class Service_Extensions3 : IContract_Extensions3
	{
		public string StringMethod(string s)
		{
			return s;
		}
	}
}
