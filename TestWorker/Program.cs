
using Dapr.Actors;
using Dapr.Actors.Client;
using WorkerManager.Actors.Contract;

namespace TestWorker
{
    public class WorkerTask
    {
        public Task Task { get; set; }
        public Guid WorkerId { get; set; }
    }
    
    public class TestWorker
    {
        private static int WorkersCount { get; } = 2;
        
        public static async Task Main(string[] args)
        {
            var managerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId("WorkerManagerActor"), "WorkerManagerActor");
            List<WorkerTask> tasks = new();
            while (true)
            {
                Console.WriteLine("task Count:" + tasks.Count);
                if (tasks.Count < WorkersCount)
                {
                    var workerId = await managerActor.StartNext();
                    if (workerId.HasValue)
                    {
                        async Task<bool> ShouldStop() =>
                            (await managerActor.GetStatus(workerId.Value)) == WorkerStatus.Stop;
                        
                        async Task SetWorkerData(WorkerData data) =>
                            await managerActor.SetWorkerData(workerId.Value, data);

                        async Task<WorkerData> GetWorkerData() =>
                            await managerActor.GetWorkerData(workerId.Value);

                        var arguments = await managerActor.GetWorkerArgs(workerId.Value);
                        
                        tasks.Add(new WorkerTask
                        {
                            WorkerId = workerId.Value,
                            Task = Task.Run(() => Run(workerId.Value, arguments, SetWorkerData, GetWorkerData, ShouldStop))
                                .ContinueWith(async t =>
                                {
                                    tasks = tasks.Where(task => task.WorkerId != workerId.Value).ToList();
                                    await managerActor.Stop(workerId.Value);
                                })
                        });
                    }
                }

                await Task.Delay(1000);
            }
        }

        private static async Task Run(Guid workerId, string[] args, Func<WorkerData, Task> setWorkerData = null,
            Func<Task<WorkerData>>? getWorkerData = null, Func<Task<bool>>? shouldStop = null)
        {
            Console.WriteLine($"Start_{workerId}");
            Console.WriteLine("Args: " + string.Join("", args));

            for (var i = 0; i < 100; i++)
            {
                try
                {
                    if (await shouldStop?.Invoke()!)
                        break;
                    
                    var data = await getWorkerData?.Invoke()!;
                    Console.WriteLine($"Task___{i}___{workerId}__status:{data.Counter}");
                    await Task.Delay(500);
                    data.Counter = i;
                    await setWorkerData?.Invoke(data)!;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
            Console.WriteLine($"End_{workerId}");
        }
        
        /*public static async Task Main(string[] args)
        {
            var workerId = Guid.Parse(args[0]);
            var controllerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId("WorkerManagerActor"), "WorkerManagerActor");

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

            var workerData = await controllerActor.GetWorkerData(workerId);
            if (workerData == null)
            {
                workerData = new WorkerData {Counter = 0};
                await controllerActor.SetWorkerData(workerId, workerData);
            }
            while (workerData.Counter <= 100)
            {
                workerData = await controllerActor.GetWorkerData(workerId);
                Console.WriteLine($"{workerData.Counter++}%");
                await controllerActor.SetWorkerData(workerId, workerData);
                
                
                if (workerData.Counter == 100)
                {
                    Console.WriteLine("done!");
                    await controllerActor.Stop(workerId);
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
        }*/
    }
}
