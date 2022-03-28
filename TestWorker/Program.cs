
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
            Console.WriteLine("Test worker started");

            var workerId = Guid.Parse(args[0]);   
            Console.WriteLine(workerId);
            var configurationActor = ActorProxy.Create<IConfigurationActor>(new ActorId("ConfigurationActor"), "ConfigurationActor");
            Console.WriteLine(1);
            var appConfiguration = await configurationActor.GetAppConfiguration("kopa");
            Console.WriteLine(appConfiguration.HelloActor);
            var helloActor = ActorProxy.Create<IMessageActor>(new ActorId($"HelloActor"), 
                appConfiguration.HelloActor);
            Console.WriteLine(appConfiguration.GoodByeActor);
            var goodByeActor = ActorProxy.Create<IMessageActor>(new ActorId($"GoodByeActor"),
                appConfiguration.GoodByeActor);
            Console.WriteLine(2);
            var helloResult = await helloActor.Execute("Test");

            var goodByeResult = await goodByeActor.Execute("Test");
            Console.WriteLine($"{helloResult}\r\n{goodByeResult}");
            
            
            var controllerActor = ActorProxy.Create<IWorkerControllerActor>(
                new ActorId("WorkerControllerActor"), "WorkerControllerActor");

            while (true)
            {
                var status = await controllerActor.GetStatus(workerId);
                Console.WriteLine($"Current worker status: {status}");
                
                
                if (status == WorkerStatus.Work)
                    break;
                
                Thread.Sleep(100);
            }
            
            
        }
    }
}
