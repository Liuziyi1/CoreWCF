using CoreWCF;
using CoreWCF.Channels;
using Helpers;
using ServiceContract;

namespace Services
{
	[ServiceBehavior]
	public class CorrelationIdVerificationServer: IHelloServer
    {
		public void SendMsg(Message m)
		{			
			string correlationId = ServiceHelper.GetCorrelationId(m);						
		}
	}
}
