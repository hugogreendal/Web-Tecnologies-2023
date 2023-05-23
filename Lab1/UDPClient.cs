using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace UPDClient
{
    class UPDClient
    {
        static void Main()
        {
            // Set the server IP address and port number
            string serverIP = "127.0.0.1";
            int serverPort = 1234;

            // Create a new UDP client and connect to the server
            UdpClient client = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

            // Create a message to send to the server
            string message = "Hello, server!";

            // Convert the message to a byte array
            byte[] byteMessage = Encoding.ASCII.GetBytes(message);

            Packet packet = new Packet();
            packet.Type = 1;
            packet.Size = byteMessage.Length;
            packet.Data = byteMessage;

            byte[] data = packet.ToByteArray();

            // Send the message to the server
            client.Send(data, data.Length, endPoint);

            // Close the client connection
            client.Close();
        }
    }
}

