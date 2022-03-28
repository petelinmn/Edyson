using System;
using Dapr.Actors;
using System.Threading.Tasks;

namespace Test.Actors.Contract
{
    public interface IWorkerControllerActor : IActor
    {
        Task<Guid> Start();
        Task<WorkerStatus> GetStatus(Guid id);
        Task Stop(Guid id);
    }
}
