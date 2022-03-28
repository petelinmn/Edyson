using System;
using Test.Actors.Contract;

namespace Test.Actors
{
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;

    public class WorkerControllerActor : Actor, IWorkerControllerActor
    {
        public async Task<Guid> Start()
        {
            Status = WorkerStatus.Work;
            return await Task.Run(() => WorkerId);
        }
        
        public async Task<WorkerStatus> GetStatus(Guid id)
        {
            return await Task.Run(() => Status);
        }
        
        public async Task Stop(Guid id)
        {
            await Task.Run(() => Status = WorkerStatus.Stop);
        }

        private Guid WorkerId { get; }
        private WorkerStatus Status { get; set; }
        private DaprClient Client { get; }

        public WorkerControllerActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            WorkerId = Guid.NewGuid();
            Status = WorkerStatus.Init;
            Client = daprClient;
        }
    }
}
