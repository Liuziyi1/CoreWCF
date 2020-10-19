using System.ServiceModel;

namespace ClientContract
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IContract_Extensions3
	{
		[OperationContract]
		string StringMethod(string s);
	}
}
