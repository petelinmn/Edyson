using System;
using System.Collections.Generic;
using System.Linq;
using WorkerManager.Actors.Contract;

namespace WorkerManager.Actors
{
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;
    
    public class WorkerManagerActor : Actor, IWorkerManagerActor
    {
        public List<Guid> WorkersQueue { get; set; } = new();
        
        public async Task<Guid> Register()
        {
            var worker = new WorkerInfo
            {
                Id = Guid.NewGuid(),
                Status = WorkerStatus.Init
            };

            await SaveWorker(worker);
            await AddWorkerToQueue(worker.Id);
            
            return worker.Id;
        }
        
        public async Task<Guid?> StartNext()
        {
            var nextWorkerId = await GetWorkerFromQueue();

            if (!nextWorkerId.HasValue)
                return null;

            var worker = await GetWorker(nextWorkerId.Value);
            
            worker.Status = WorkerStatus.Work;
            
            await SaveWorker(worker);

            return nextWorkerId;

            //new event OnStart Worker
        }
        
        public async Task<WorkerStatus> GetStatus(Guid id)
        {
            var worker = await GetWorker(id);
            return worker.Status;
        }
        
        public async Task Stop(Guid id)
        {
            var worker = await GetWorker(id);

            worker.Status = WorkerStatus.Stop;

            await SaveWorker(worker);

            //new event OnStop Worker
        }

        public async Task SetWorkerData(Guid id, WorkerData data)
        {
            var worker = await GetWorker(id);

            worker.Data = data;

            await SaveWorker(worker);
        }

        public async Task<WorkerData> GetWorkerData(Guid id)
        {
            var worker = await GetWorker(id);
            return worker.Data ?? new WorkerData();
        }
        
        private string GetWorkerStateKey(string key) => $"worker_{key}";
        private string QueueToWorkStateKey { get; } = $"queueToWorkStateKey";
        private readonly string StoreName = "statestore";
        private Guid WorkerId { get; }
        private WorkerStatus Status { get; set; }
        private DaprClient Client { get; }

        private async Task<WorkerInfo> GetWorker(Guid Id)
        {
            var worker = await Client.GetStateAsync<WorkerInfo>(StoreName, GetWorkerStateKey(Id.ToString()));
            if (worker == null)
            {
                throw new Exception("Worker not found");
            }

            return worker;
        }

        private async Task SaveWorker(WorkerInfo worker) =>
            await Client.SaveStateAsync(StoreName, GetWorkerStateKey(worker.Id.ToString()), worker);

        
        private async Task AddWorkerToQueue(Guid id)
        {
            var key = GetWorkerStateKey(QueueToWorkStateKey);
            var queue = await Client.GetStateAsync<Queue<Guid>>(StoreName, key) ??
                        new Queue<Guid>();
            
            queue.Enqueue(id);

            await Client.SaveStateAsync(StoreName, key, queue);
        }
        
        private async Task<Guid?> GetWorkerFromQueue()
        {
            var key = GetWorkerStateKey(QueueToWorkStateKey);
            var queue = await Client.GetStateAsync<Queue<Guid>>(StoreName, key) ??
                        new Queue<Guid>();
            
            var workerId = queue.Count > 0 ? queue.Dequeue() : (Guid?)null;

            await Client.SaveStateAsync(StoreName, key, queue);
            
            return workerId;
        }
        
        public WorkerManagerActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            WorkerId = Guid.NewGuid();
            Status = WorkerStatus.Init;
            Client = daprClient;
        }
    }
}
