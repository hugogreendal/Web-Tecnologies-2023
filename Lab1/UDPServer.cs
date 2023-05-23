using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace UPDServer
{
    class UPDServer
    {
        static void Main()
        {
            // Set the local IP address and port number to listen on
            IPAddress localIP = IPAddress.Parse("127.0.0.1");
            int localPort = 1234;

            // Create a new UDP listener
            UdpClient listener = new UdpClient(localPort);

            // Create an endpoint to listen for incoming messages
            IPEndPoint endPoint = new IPEndPoint(localIP, localPort);

            Stopwatch stopwatch = new();
            try
            {
                // Start listening for incoming messages
                Console.WriteLine("Waiting for messages...");
                byte[] data = listener.Receive(ref endPoint);
                stopwatch.Start();

                Packet packet = Packet.FromByteArray(data);
                int type = packet.Type;
                int size = packet.Size;
                byte[] receivedData = packet.Data;

                // Convert the received data to a string
                string message = Encoding.ASCII.GetString(data);

                // Display the received message
                Console.WriteLine("Received message: " + message);
                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                // Close the listener
                listener.Close();
            }
        }
    }
}
