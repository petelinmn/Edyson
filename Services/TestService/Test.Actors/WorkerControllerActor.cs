using System;
using Test.Actors.Contract;

namespace Test.Actors
{
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;

    public class WorkerData
    {
        public Guid Id { get; set; }
        public WorkerStatus Status { get; set; }
        
        public int Counter { get; set; }
    }
    
    public class WorkerControllerActor : Actor, IWorkerControllerActor
    {
        public async Task<Guid> Register()
        {
            var worker = new WorkerData
            {
                Id = Guid.NewGuid(),
                Status = WorkerStatus.Init
            };

            await SaveWorker(worker);
            return worker.Id;
        }
        
        public async Task Start(Guid Id)
        {
            var worker = await GetWorker(Id);

            worker.Status = WorkerStatus.Work;
            
            await SaveWorker(worker);
            
            //new event OnStart Worker
        }
        
        public async Task<WorkerStatus> GetStatus(Guid Id)
        {
            var worker = await GetWorker(Id);
            return worker.Status;
        }
        
        public async Task Stop(Guid Id)
        {
            var worker = await GetWorker(Id);

            worker.Status = WorkerStatus.Stop;

            await SaveWorker(worker);

            //new event OnStop Worker
        }

        public async Task SetCounter(Guid Id, int counter)
        {
            var worker = await GetWorker(Id);

            worker.Counter = counter;

            await SaveWorker(worker);
        }

        public async Task<int> GetCounter(Guid Id)
        {
            var worker = await GetWorker(Id);
            return worker.Counter;
        }
        
        private string GetWorkerStateKey(Guid key) => $"worker_{key}";
        private readonly string StoreName = "statestore";
        private Guid WorkerId { get; }
        private WorkerStatus Status { get; set; }
        private DaprClient Client { get; }

        private async Task<WorkerData> GetWorker(Guid Id)
        {
            var worker = await Client.GetStateAsync<WorkerData>(StoreName, GetWorkerStateKey(Id));
            if (worker == null)
            {
                throw new Exception("Worker not found");
            }

            return worker;
        }

        private async Task SaveWorker(WorkerData worker) =>
            await Client.SaveStateAsync(StoreName, GetWorkerStateKey(worker.Id), worker);
        
        public WorkerControllerActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            WorkerId = Guid.NewGuid();
            Status = WorkerStatus.Init;
            Client = daprClient;
        }
    }
}
