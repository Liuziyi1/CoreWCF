using CoreWCF;

namespace ServiceContract
{
    [ServiceContract]
    public interface ICheckSPN
    {
        [OperationContract]
        bool CheckSPN();
    }
}
