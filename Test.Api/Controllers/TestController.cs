using Configuration.Actors;
using Configuration.Actors.Contract;
using Dapr.Client;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Dapr;
using Microsoft.AspNetCore.Cors;
using Test.Actors.Contract;

namespace Test.Api.Controllers
{
    public class TestRequest
    {
        public string? Name { get; set; }
    }
    
    
    [ApiController]
    [Route("api/v1")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [Route("test")]
        [HttpPost]
        public async Task<string?> Execute([FromBody] TestRequest request)
        {
            var configurationActor = ActorProxy.Create<IConfigurationActor>(new ActorId("ConfigurationActor"), "ConfigurationActor");
            var appConfiguration = await configurationActor.GetAppConfiguration("kopa");

            var helloActor = ActorProxy.Create<IMessageActor>(new ActorId($"HelloActor"), 
                appConfiguration.HelloActor);

            var goodByeActor = ActorProxy.Create<IMessageActor>(new ActorId($"GoodByeActor"), 
                appConfiguration.GoodByeActor);

            var helloResult = await helloActor.Execute(request.Name);

            var goodByeResult = await goodByeActor.Execute(request.Name);
            return $"{helloResult}\r\n{goodByeResult}";
        }
    }
}
