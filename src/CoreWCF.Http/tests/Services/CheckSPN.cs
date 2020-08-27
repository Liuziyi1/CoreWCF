using CoreWCF;
using CoreWCF.Channels;
using ServiceContract;
using System;

namespace Services
{
    public class CheckSPN : ICheckSPN
    {
        bool ICheckSPN.CheckSPN()
        {
            MessageProperties properties = OperationContext.Current.IncomingMessageProperties;

            if (!properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                throw new ArgumentException("Input message does not contain property '{0}'.", HttpRequestMessageProperty.Name);
            }

            HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)properties[HttpRequestMessageProperty.Name];
            foreach (string key in requestProperty.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}",
                    key,
                    requestProperty.Headers[key]);
            }

            return true;
        }
    }
}
