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
    public class WorkerRequest
    {
        public Guid WorkerId { get; set; }
    }
    
    [ApiController]
    [Route("api/worker")]
    public class WorkerController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public WorkerController(ILogger<TestController> logger)
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
        
        [Route("register-worker")]
        [HttpPost]
        public async Task<Guid> RegisterWorker()
        {
            var workerControllerActor = ActorProxy.Create<IWorkerControllerActor>(
                new ActorId("WorkerControllerActor"), "WorkerControllerActor");

            var guid = await workerControllerActor.Register();
            return guid;
        }
        
        [Route("start-worker")]
        [HttpPost]
        public async Task StartWorker([FromBody] WorkerRequest request)
        {
            var workerControllerActor = ActorProxy.Create<IWorkerControllerActor>(
                new ActorId("WorkerControllerActor"), "WorkerControllerActor");

            await workerControllerActor.Start(request.WorkerId);
        }
        
        [Route("stop-worker")]
        [HttpPost]
        public async Task StopWorker([FromBody] WorkerRequest request)
        {
            var workerControllerActor = ActorProxy.Create<IWorkerControllerActor>(
                new ActorId("WorkerControllerActor"), "WorkerControllerActor");

            await workerControllerActor.Stop(request.WorkerId);
        }
    }
}
