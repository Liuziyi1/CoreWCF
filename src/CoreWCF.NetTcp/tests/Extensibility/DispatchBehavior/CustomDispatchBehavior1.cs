using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreWCF.NetTcp.Tests.Extensibility.DispatchBehavior
{
	public class CustomDispatchBehavior1 : IContractBehavior, IOperationBehavior
	{
		void IContractBehavior.AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection parameters)
		{
		}

		void IContractBehavior.ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime proxy)
		{
		}

		void IContractBehavior.Validate(ContractDescription description, ServiceEndpoint endpoint)
		{
		}

		void IContractBehavior.ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatch)
		{
            //switch (ExtensibilityTests.currentVariationType)
            //{
            //    case VariationType1.channelinitializers:
            //        dispatch.ChannelDispatcher.ChannelInitializers.Add(new CustomChannelInitializer3());
            //        dispatch.ChannelDispatcher.ChannelInitializers.Add(new CustomChannelInitializer3());
            //        dispatch.ChannelDispatcher.ChannelInitializers.Add(new OtherCustomChannelInitializer3());
            //        return;
            //    case VariationType1.messageinspectors:
            //        dispatch.MessageInspectors.Add(new CustomMessageInspector3());
            //        dispatch.MessageInspectors.Add(new CustomMessageInspector3());
            //        dispatch.MessageInspectors.Add(new OtherCustomMessageInspector3());
            //        return;
            //    case VariationType1.messageinspectorsusinghiddenproperty:
            //    case VariationType1.parameterinspectors:
            //    case VariationType1.parameterinspectorsusinghiddenproperty:
            //    case VariationType1.addingstatechannelinitializer:
            //        break;
            //    case VariationType1.instancecontextinitializers:
            //        dispatch.InstanceContextInitializers.Add(new CustomInstanceContextInitializer3());
            //        dispatch.InstanceContextInitializers.Add(new CustomInstanceContextInitializer3());
            //        dispatch.InstanceContextInitializers.Add(new OtherCustomInstanceContextInitializer3());
            //        return;
            //    case VariationType1.operationselector:
            //        dispatch.OperationSelector = new CustomOperationSelector3();
            //        break;
            //    case VariationType1.inputsessionshutdownhandlers:
            //        dispatch.InputSessionShutdownHandlers.Add(new CustomInputSessionShutdownHandler());
            //        dispatch.InputSessionShutdownHandlers.Add(new CustomInputSessionShutdownHandler());
            //        dispatch.InputSessionShutdownHandlers.Add(new OtherCustomInputSessionShutdownHandler());
            //        return;
            //    case VariationType1.faultedinputsessionshutdownhandlers:
            //        dispatch.InputSessionShutdownHandlers.Add(new NonDeterminsticInputSessionShutdownHandler());
            //        return;
            //    case VariationType1.instanceprovider:
            //        dispatch.InstanceProvider = new CustomInstanceProvider3();
            //        return;
            //    case VariationType1.addingstateinstancecontextinitializer:
            //        dispatch.InstanceContextInitializers.Add(new AddingStateInstanceContextInitializer3());
            //        dispatch.InstanceProvider = new VerifyingInstanceContextStateInstanceProvider3();
            //        return;
            //    default:
            //        return;
            //}
        }

		void IOperationBehavior.AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
		{
		}

		void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
		{
		}

		void IOperationBehavior.Validate(OperationDescription description)
		{
		}

		void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
		{
			//VariationType1 currentVariationType = Extensions_VariationHandler.currentVariationType;
			//if (currentVariationType == VariationType1.parameterinspectors)
			//{
			//	dispatch.ParameterInspectors.Add(new CustomParameterInspector3());
			//	dispatch.ParameterInspectors.Add(new CustomParameterInspector3());
			//	dispatch.ParameterInspectors.Add(new OtherCustomParameterInspector3());
			//}
		}
	}
}
