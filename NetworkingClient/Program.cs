using System;
using System.Threading.Tasks;

namespace Client
{
    internal class Start
    {
        class Program
        {
            static async Task Main(string[] args)
            {
                await Sender.RunClient(12345, 12346);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
