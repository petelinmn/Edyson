using System;
using Dapr.Actors;
using System.Threading.Tasks;

namespace Test.Actors.Contract
{
    public interface IWorkerControllerActor : IActor
    {
        Task<Guid> Register();
        Task Start(Guid id);
        Task<WorkerStatus> GetStatus(Guid id);
        Task Stop(Guid id);
        Task SetCounter(Guid id, int counter);
        Task<int> GetCounter(Guid id);
    }
}
