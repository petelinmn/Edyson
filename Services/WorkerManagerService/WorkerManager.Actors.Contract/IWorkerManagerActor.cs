using System;
using Dapr.Actors;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WorkerManager.Actors.Contract
{
    public interface IWorkerManagerActor : IActor
    {
        Task<Guid> Register();
        Task<Guid?> StartNext();
        Task<WorkerStatus> GetStatus(Guid id);
        Task Stop(Guid id);
        Task SetWorkerData(Guid id, WorkerData data);
        Task<WorkerData> GetWorkerData(Guid id);
    }
}
