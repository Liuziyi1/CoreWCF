using CoreWCF.Configuration;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ServiceContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.Http.Tests
{
    public class SPNVerificationTests
    {
        private ITestOutputHelper _output;
        public SPNVerificationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("Default")]
        [InlineData("HTTP")]
        [InlineData("HOST")]
        public void SPNVerificationTest(string SPNFormat)
        {
            _output.WriteLine("Start Testing...");

            TraceSource nclTraceSource = GetNCLTraceSource("Web", _output);
            nclTraceSource.Listeners.Clear();
            nclTraceSource.Switch.Level = SourceLevels.All;
            _output.WriteLine("Get NCL Trace Source...");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            TextWriterTraceListener listener = new TextWriterTraceListener(sw);

            nclTraceSource.Listeners.Add(listener);
            _output.WriteLine("Add NCL trace listener...");

            System.ServiceModel.BasicHttpBinding basichttpbinding = new System.ServiceModel.BasicHttpBinding();
            basichttpbinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
            basichttpbinding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
            _output.WriteLine("Create BasicHttpBinding...");

            string fqdn = GetFQDN();
            string listenUri = string.Format("http://{0}:8088/BasicWcfService/basichttp.svc", fqdn);
            _output.WriteLine("Listening at {0} ...", listenUri);

            var host = ServiceHelper.CreateWebHostBuilder<SPNStartup>(_output, string.Format("http://{0}:8088", fqdn)).Build();
            using (host)
            {
                host.Start();
                

#if NET472
                System.ServiceModel.EndpointIdentity hostEndpointIdentity = System.ServiceModel.EndpointIdentity.CreateSpnIdentity(String.Format("HOST/{0}", System.Net.Dns.GetHostEntry(String.Empty).HostName));
                System.ServiceModel.EndpointIdentity httpEndpointIdentity = System.ServiceModel.EndpointIdentity.CreateSpnIdentity(String.Format("HTTP/{0}", System.Net.Dns.GetHostEntry(String.Empty).HostName));
#endif
                string expectedSPNFormat = "HTTP";
                _output.WriteLine("Creating channel factory with {0} SPN format...", SPNFormat);
                System.ServiceModel.ChannelFactory<ClientContract.ICheckSPN> factory = null;
                switch (SPNFormat)
                {
                    case "Default":
                        factory = new System.ServiceModel.ChannelFactory<ClientContract.ICheckSPN>(basichttpbinding, new System.ServiceModel.EndpointAddress(listenUri));
                        break;
                    case "HTTP":
#if NET472
                        factory = new System.ServiceModel.ChannelFactory<ClientContract.ICheckSPN>(basichttpbinding, new System.ServiceModel.EndpointAddress(new Uri(listenUri), httpEndpointIdentity));
#endif
                        break;
                    case "HOST":
#if NET472
                        factory = new System.ServiceModel.ChannelFactory<ClientContract.ICheckSPN>(basichttpbinding, new System.ServiceModel.EndpointAddress(new Uri(listenUri), hostEndpointIdentity));
                        expectedSPNFormat = "HOST";
#endif
                        break;
                    default:
                        break;
                }
                factory.Open();
                _output.WriteLine("Channel factory opened...");

                ClientContract.ICheckSPN channel = factory.CreateChannel();
                var t = channel.CheckSPN();
                _output.WriteLine("ReturnValue: {0}", channel.CheckSPN());

                string[] spns = GetSPN(sb.ToString(), _output);

                if (spns != null && spns.Length > 0)
                {
                    _output.WriteLine("SPNs used: ");
                    foreach (string spn in spns)
                    {
                        _output.WriteLine("    - {0}", spn);

                        Assert.True(spn.StartsWith(expectedSPNFormat, StringComparison.InvariantCultureIgnoreCase), string.Format("Expected SPN Format: {0}; Actual SPN Format: {1}", expectedSPNFormat, spn));                        
                    }
                }

                factory.Close();
            }
        }

        private static TraceSource GetNCLTraceSource(string name, ITestOutputHelper output)
        {
            Assembly nclAssembly = Assembly.GetAssembly(typeof(System.Net.HttpWebRequest));
            object logging = nclAssembly.CreateInstance("System.Net.Logging", false, BindingFlags.Instance | BindingFlags.NonPublic, null, null, null, null);
            Type loggingType = logging.GetType();

            FieldInfo tracingEnabledInfo = loggingType.GetField("s_LoggingEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo tracingInitializedInfo = loggingType.GetField("s_LoggingInitialized", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo traceSourceInfo = loggingType.GetField(string.Format("s_{0}TraceSource", name), BindingFlags.Static | BindingFlags.NonPublic);

            output.WriteLine("Logging Initialized: {0}", tracingInitializedInfo.GetValue(logging));
            output.WriteLine("Logging Enabled: {0}", tracingEnabledInfo.GetValue(logging));
            output.WriteLine("TraceSource: {0}", traceSourceInfo.GetValue(logging));

            PropertyInfo webInfo = loggingType.GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic);
            TraceSource nclTraceSource = (TraceSource)webInfo.GetValue(logging, BindingFlags.Static | BindingFlags.NonPublic, null, null, null);

            if (nclTraceSource == null)
            {
                output.WriteLine("Logging Initialized: {0}", tracingInitializedInfo.GetValue(logging));
                output.WriteLine("Logging Enabled: {0}", tracingEnabledInfo.GetValue(logging));
                output.WriteLine("TraceSource: {0}", traceSourceInfo.GetValue(logging));

                tracingEnabledInfo = loggingType.GetField("s_LoggingEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                tracingEnabledInfo.SetValue(logging, true);

                output.WriteLine("Logging Initialized: {0}", tracingInitializedInfo.GetValue(logging));
                output.WriteLine("Logging Enabled: {0}", tracingEnabledInfo.GetValue(logging));
                output.WriteLine("TraceSource: {0}", traceSourceInfo.GetValue(logging));

                webInfo = loggingType.GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic);
                nclTraceSource = (TraceSource)webInfo.GetValue(logging, BindingFlags.Static | BindingFlags.NonPublic, null, null, null);
            }

            if (nclTraceSource == null)
            {
                throw new ArgumentNullException("nclTraceSOurce");
            }

            return nclTraceSource;
        }

        private static string[] GetSPN(string text, ITestOutputHelper output)
        {
            string pattern = ".*InitializeSecurityContext.*targetName...(.*),.inFlags";
            Regex regex = new Regex(pattern);

            List<string> spns = new List<string>();

            foreach (Match match in regex.Matches(text))
            {
                output.WriteLine("Found one match: {0}", match.Groups[1].Value);
                spns.Add(match.Groups[1].Value);
            }

            return spns.ToArray();
        }

        public static string GetFQDN()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            if (!string.IsNullOrEmpty(ipGlobalProperties.DomainName))
            {
                return string.Format("{0}.{1}", ipGlobalProperties.HostName, ipGlobalProperties.DomainName);
            }

            return ipGlobalProperties.HostName;
        }

        internal class SPNStartup
        {
            public static string _method;

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseServiceModel(builder =>
                {

                    builder.AddService<Services.CheckSPN>();
                    builder.AddServiceEndpoint<Services.CheckSPN, ICheckSPN>(ServiceHelper.GetBindingWithCredential() , "/BasicWcfService/basichttp.svc");

                });
            }
        }
    }
}
