using System.ServiceModel;

namespace ClientContract
{
    [ServiceContract]
    public interface ICheckSPN
    {
        [OperationContract]
        bool CheckSPN();
    }
}
