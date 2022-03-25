using Test.Actors.Contract;

namespace Test.Actors
{
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;

    public class GoodByeFromJaneActor : Actor, IMessageActor
    {
        public async Task<string> Execute(string name)
        {
            return await Task.Run(() => $@"GoodBye from Jane, {name}");
        }

        private DaprClient Client { get; }

        public GoodByeFromJaneActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            Client = daprClient;
        }
    }
}
