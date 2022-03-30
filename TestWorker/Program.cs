
using Configuration.Actors.Contract;
using Dapr.Actors;
using Dapr.Actors.Client;
using Test.Actors.Contract;

namespace TestWorker
{
    public class TestWorker
    {
        public static async Task Main(string[] args)
        {
            var workerId = Guid.Parse(args[0]);
            var controllerActor = ActorProxy.Create<IWorkerControllerActor>(
                new ActorId("WorkerControllerActor"), "WorkerControllerActor");

            var status = await controllerActor.GetStatus(workerId);
            var previousStatus = status;

            if (status == WorkerStatus.Init)
            {
                Console.WriteLine("Test worker started");
            }
            
            while (true)
            {
                status = await controllerActor.GetStatus(workerId);
                if (previousStatus != status)
                {
                    Console.WriteLine($"Current worker status: {status}");
                    previousStatus = status;
                }

                if (status != WorkerStatus.Init)
                    break;

                await Task.Delay(100);
            }

            var workCounter = await controllerActor.GetCounter(workerId);
            while (workCounter <= 100)
            {
                workCounter = await controllerActor.GetCounter(workerId);
                Console.WriteLine($"{workCounter++}%");
                await controllerActor.SetCounter(workerId, workCounter);
                
                
                if (workCounter == 100)
                {
                    Console.WriteLine("done!");
                    break;
                }
                
                status = await controllerActor.GetStatus(workerId);
                if (previousStatus != status)
                {
                    Console.WriteLine($"Current worker status: {status}");
                    previousStatus = status;
                }

                if (status == WorkerStatus.Stop)
                {
                    Console.WriteLine("Worker is stopped");
                    break;
                }
                
                Thread.Sleep(300);
            }

        }
    }
}
