using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Simjob.Framework.Services.Api;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Simjob.Framework.Test.Context
{
    [ExcludeFromCodeCoverage]
    public class TestContext
    {
        public HttpClient Client { get; set; }
        private TestServer _server;

        public TestContext()
        {
            SetupClient();
        }

        [ExcludeFromCodeCoverage]
        private void SetupClient()
        {
            //var ok = "c8c3d77960c19109d5bfbe67c06ed0fcdf02440ec3d1120a979ed0c3b3ae76be";
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseTestServer()
                .ConfigureTestServices(collection =>
                   {
                       collection.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                   })
                );
            Client = _server.CreateClient();
        }
    }
}