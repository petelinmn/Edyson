using Configuration.Actors;
using Configuration.Actors.Contract;
using Dapr.Client;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Dapr;
using Microsoft.AspNetCore.Cors;
using Test.Actors.Contract;
using WorkerManager.Actors.Contract;

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
        private readonly ILogger<WorkerController> _logger;

        public WorkerController(ILogger<WorkerController> logger)
        {
            _logger = logger;
        }
        
        [Route("register-worker")]
        [HttpPost]
        public async Task<Guid> RegisterWorker([FromBody] string[] args)
        {
            var workerControllerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId("WorkerManagerActor"), "WorkerManagerActor");

            return await workerControllerActor.Register(args);
        }
        
        [Route("stop-worker")]
        [HttpPost]
        public async Task StopWorker([FromBody] WorkerRequest request)
        {
            var workerControllerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId("WorkerManagerActor"), "WorkerManagerActor");

            await workerControllerActor.Stop(request.WorkerId);
        }
    }
}
