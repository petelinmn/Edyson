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
    public class ConfigurationSetRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    
    [ApiController]
    [Route("configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public ConfigurationController(ILogger<TestController> logger)
        {
            _logger = logger;
        }
        
        [Route("{key}")]
        [HttpGet]
        public async Task<string> Get([FromRoute] string key)
        {
            var actor = ActorProxy.Create<IConfigurationActor>(new ActorId($"Configuration_getter"), 
                "ConfigurationActor");
            
            return await actor.Get(key);
        }
        
        [Route("app/{key}")]
        [HttpGet]
        public async Task<AppConfiguration> GetAppConfiguration([FromRoute] string key)
        {
            var actor = ActorProxy.Create<IConfigurationActor>(new ActorId($"Configuration_getter"), 
                "ConfigurationActor");
            
            return await actor.GetAppConfiguration(key);
        }

        [HttpPost]
        public async Task Set([FromBody] ConfigurationSetRequest request)
        {
            var actor = ActorProxy.Create<IConfigurationActor>(new ActorId($"Configuration_setter_{request.Key ?? "unknown"}"), 
                "ConfigurationActor");
            
            await actor.Set(request.Key, request.Value);
        }
        
        [Route("app")]
        [HttpPost]
        public async Task SetKopaConfiguration([FromBody] AppConfiguration request)
        {
            var actor = ActorProxy.Create<IConfigurationActor>(new ActorId($"Configuration_setter_kopa"), 
                "ConfigurationActor");
            
            await actor.SetAppConfiguration("kopa", request);
        }
    }
}
