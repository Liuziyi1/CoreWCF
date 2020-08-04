using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClientContract
{
    [ServiceContract]
	public interface IHelloServer
	{
		[OperationContract(Action = "*")]
		void SendMsg(Message m);
	}
}
