using System.Net;
using System.Net.Sockets;
using System.Text;
using NetworkingCommon;

namespace NetworkingServer
{
    public class UdpServer
    {
        private readonly int receivePort;
        private readonly int sendPort;
        private UdpClient udpClient;

        public UdpServer(int receivePort, int sendPort)
        {
            this.receivePort = receivePort;
            this.sendPort = sendPort;
            udpClient = new UdpClient(receivePort);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Waiting for message...");

                    var receiveTask = udpClient.ReceiveAsync();

                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(-1, cancellationToken));

                    if (completedTask == receiveTask)
                    {
                        var receiveResult = receiveTask.Result;
                        var jsonMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
                        var message = Message.DeserializeFromJson(jsonMessage);
                        Console.WriteLine("Message received:");
                        Console.WriteLine(message);

                        var senderEndPoint = receiveResult.RemoteEndPoint;
                        await UdpHelper.SendAsync(message, (IPEndPoint)senderEndPoint);

                        Console.WriteLine("Message sent back to client.");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Server stopped.");
            }
            finally
            {
                udpClient.Dispose();
            }
        }
    }
}
