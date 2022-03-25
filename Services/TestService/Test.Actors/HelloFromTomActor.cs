using Test.Actors.Contract;

namespace Test.Actors
{
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;

    public class HelloFromTomActor : Actor, IMessageActor
    {
        public async Task<string> Execute(string name)
        {
            return await Task.Run(() => $@"Hello from Tom, {name}");
        }

        private DaprClient Client { get; }

        public HelloFromTomActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            Client = daprClient;
        }
    }
}
