using System;
using Dapr.Actors;
using System.Threading.Tasks;

namespace Test.Actors.Contract
{
    public enum WorkerStatus
    {
        Init,
        Work,
        Stop
    }
}
