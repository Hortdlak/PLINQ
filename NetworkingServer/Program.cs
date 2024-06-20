namespace NetworkingServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new UdpServer(12345, 12346);
            await server.StartAsync();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
