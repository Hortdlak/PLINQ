using System.Net;
using System.Net.Sockets;
using System.Text;
using NetworkingCommon;
namespace Client
{
    public class Sender
    {
        public static async Task Send(Message msg, int port, string address = "127.0.0.1")
        {
            using var udpClient = new UdpClient();
            var endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            var jsonMessage = msg.SerializeToJson();
            var buffer = Encoding.UTF8.GetBytes(jsonMessage);

            for (int i = 0; i < 3; i++)
                await udpClient.SendAsync(buffer, buffer.Length, endPoint);

            Console.WriteLine("Message sent");
        }

        public static async Task<Message?> Receive(int port, string address = "127.0.0.1")
        {
            using var udpClient = new UdpClient(port);
            var endPoint = new IPEndPoint(IPAddress.Parse(address), 0);

            Console.WriteLine("Waiting for connection");
            udpClient.Connect(endPoint);
            Console.WriteLine("Connected");
            Console.WriteLine("Receiving message");

            var data = await udpClient.ReceiveAsync();
            var buffer = data.Buffer;
            var jsonMessage = Encoding.UTF8.GetString(buffer);
            var message = Message.DeserializeFromJson(jsonMessage);

            Console.WriteLine("Message received.");
            Console.WriteLine(message);

            return message;
        }

        public static async Task RunClient(int sendPort, int receivePort)
        {
            try
            {
                var serverAddress = "127.0.0.1";

                Console.WriteLine("Enter message or 'exit'");
                string input;
                do
                {
                    input = Console.ReadLine();
                    if (!input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        var msg = new Message(input, "Client", "Server");
                        await Send(msg, sendPort, serverAddress);

                        var response = await Receive(receivePort, serverAddress);
                        if (response?._text?.Equals("server off", StringComparison.OrdinalIgnoreCase) ?? false)
                            break;
                    }
                } while (!input.Equals("exit", StringComparison.OrdinalIgnoreCase));

                Console.WriteLine("Client off.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
