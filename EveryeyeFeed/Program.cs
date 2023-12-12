using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace EveryeyeFeed
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            await host.RunAsync();
        }
    }
}
