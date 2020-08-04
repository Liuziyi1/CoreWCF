using CoreWCF.Configuration;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract;
using System;
using System.ServiceModel.Channels;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.Http.Tests
{
   public class PropagationPointsTests
    {
        private ITestOutputHelper _output;
        public PropagationPointsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CorrelationIdVerification()
        {
            var host = ServiceHelper.CreateWebHostBuilder<PropagationPointsStartup>(_output).Build();
            using (host)
            {
                host.Start();
                var clientProxy = ClientHelper.GetProxy<ClientContract.IHelloServer>();
                Message message = Message.CreateMessage(MessageVersion.Soap11, "http://tempuri.org/ITestContract/SendMsg");
                 clientProxy.SendMsg(message);
                string correlationId = ClientHelper.GetCorrelationId(message);
                //Assert.True(result == correlationId);
            }
        }

        internal class PropagationPointsStartup
        {           
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseServiceModel(builder =>
                {                    
                            builder.AddService<Services.CorrelationIdVerificationServer>();
                            builder.AddServiceEndpoint<Services.CorrelationIdVerificationServer, IHelloServer>(new CoreWCF.BasicHttpBinding(), "/BasicWcfService/basichttp.svc");
                      
                    
                });
            }
        }
    }
}
