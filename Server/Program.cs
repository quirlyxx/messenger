using Server.Network;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ServerP();
            await server.StartAsync("127.0.0.1", 8888);
        }
    }
}
