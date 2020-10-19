using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.IdentityModel.Policy;
using CoreWCF.Primitives.Tests.CustomSecurity;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;


namespace CoreWCF.NetTcp.Tests
{
  public class ExtensibilityTests
    {
        private ITestOutputHelper _output;
        public static VariationType1 currentVariationType;
        public ExtensibilityTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("Dispatcher", "channelinitializers")]
        [InlineData("proxy", "channelinitializers")]
        [InlineData("proxy", "addingstatechannelinitializer")]
        [InlineData("dispatcher", "messageinspectors")]
        [InlineData("proxy", "messageinspectors")]
        [InlineData("proxy", "messageinspectorsusinghiddenproperty")]
        [InlineData("dispatcher", "InstanceContextInitializers")]
        [InlineData("dispatcher", "AddingStateInstanceContextInitializer")]

        [InlineData("dispatcher", "InstanceProvider")]
        [InlineData("dispatcher", "InputSessionShutdownHandlers")]
        [InlineData("dispatcher", "FaultedInputSessionShutdownHandlers")]
        [InlineData("proxy", "ParameterInspectors")]
        [InlineData("proxy", "parameterinspectorsusinghiddenproperty")]
        [InlineData("Dispatcher", "ParameterInspectors")]
        [InlineData("proxy", "operationselector")]
        [InlineData("Dispatcher", "operationselector")]
        public void ProxyAndDispatchRuntime(string location,string variationType)
        {
            ExtensibilityTests.currentVariationType = (VariationType1)Enum.Parse(typeof(VariationType1), variationType, true);
            var host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                string testString = "Hello";
                System.ServiceModel.ChannelFactory<ClientContract.IContract_Extensions3> factory = null;
                ClientContract.IContract_Extensions3 channel = null;
                host.Start();
                try
                {
                    var binding = ClientHelper.GetBufferedModeBinding(System.ServiceModel.SecurityMode.None);
                    factory = new System.ServiceModel.ChannelFactory<ClientContract.IContract_Extensions3>(binding, new System.ServiceModel.EndpointAddress(new Uri("net.tcp://localhost:8808//nettcp.svc/security-none")));
                    channel = factory.CreateChannel();
                    ((IChannel)channel).Open();
                    var result = channel.StringMethod(testString);
                    Assert.Equal(testString, result);
                    ((IChannel)channel).Close();
                    factory.Close();
                }
                finally
                {
                    ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
                }
            }
        }
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {              
                app.UseServiceModel(builder =>
                {
                    builder.AddService<Services.TestService>();
                    builder.AddServiceEndpoint<Services.Service_Extensions3, ServiceContract.IContract_Extensions3>(new CoreWCF.NetTcpBinding(), "/nettcp.svc/security-none");
                });
            }
        }
    }

    public enum VariationType1
    {
        channelinitializers,
        messageinspectors,
        messageinspectorsusinghiddenproperty,
        parameterinspectors,
        parameterinspectorsusinghiddenproperty,
        instancecontextinitializers,
        operationselector,
        inputsessionshutdownhandlers,
        faultedinputsessionshutdownhandlers,
        instanceprovider,
        addingstatechannelinitializer,
        addingstateinstancecontextinitializer,
        operationinvoker
    }
}
