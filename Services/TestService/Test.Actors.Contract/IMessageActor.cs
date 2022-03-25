using Dapr.Actors;
using System.Threading.Tasks;

namespace Test.Actors.Contract
{
    public interface IMessageActor : IActor
    {
        Task<string> Execute(string name);
    }
}
