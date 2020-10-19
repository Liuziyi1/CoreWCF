using CoreWCF;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceContract
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IContract_Extensions3
	{
		[OperationContract]
		string StringMethod(string s);
	}
}
