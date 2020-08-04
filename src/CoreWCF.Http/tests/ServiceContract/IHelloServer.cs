using CoreWCF;
using CoreWCF.Channels;

namespace ServiceContract
{
    [ServiceContract]
	public interface IHelloServer
	{
		[OperationContract(Action = "*")]
		void SendMsg(Message m);
	}
}
