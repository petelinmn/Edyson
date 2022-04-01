using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;

namespace WorkerManager.Actors.Contract
{
    public abstract class Worker
    {
        public abstract Task Entry();

        private Guid Id { get; set; }

        public async Task Run(Guid id)
        {
            Id = id;
            var tasks = new List<Task>
            {
                Task.Run(Flow),
                Task.Run(Entry)
            };

            try
            {
                Console.WriteLine("when all start");
                await Task.WhenAll(tasks.ToArray());
                Console.WriteLine("when all stop");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void Flow()
        {
            var controllerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId("WorkerManagerActor"), "WorkerManagerActor");

            var status = await controllerActor.GetStatus(Id);
            var previousStatus = status;

            if (status == WorkerStatus.Init)
            {
                Console.WriteLine("Test worker started");
            }
            
            while (true)
            {
                status = await controllerActor.GetStatus(Id);
                if (previousStatus != status)
                {
                    Console.WriteLine($"Current worker status: {status}");
                    previousStatus = status;
                }

                if (status != WorkerStatus.Init)
                    break;

                await Task.Delay(100);
            }

        }
    }
}
