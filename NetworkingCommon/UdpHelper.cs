using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingCommon
{
    public static class UdpHelper
    {
        public static async Task SendAsync(Message msg, IPEndPoint endPoint)
        {
            using var udpClient = new UdpClient();
            var jsonMessage = msg.SerializeToJson();
            var buffer = Encoding.UTF8.GetBytes(jsonMessage);

            for (int i = 0; i < 3; i++)
                await udpClient.SendAsync(buffer, buffer.Length, endPoint);

            Console.WriteLine("Message sent");
        }

        public static async Task<Message?> ReceiveAsync(int port, System.Threading.CancellationToken cancellationToken = default)
        {
            using var udpClient = new UdpClient(port);
            try
            {
                Console.WriteLine("Waiting for connection");
                var data = await udpClient.ReceiveAsync(cancellationToken);
                var jsonMessage = Encoding.UTF8.GetString(data.Buffer);
                var message = Message.DeserializeFromJson(jsonMessage);

                Console.WriteLine("Message received.");
                Console.WriteLine(message);

                return message;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation canceled!");
                return null;
            }
        }
    }
}
