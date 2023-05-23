using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TcpServer
{
    private static Boolean isRunning = true;
    private static int connectionsCount = 0;

    static async Task Main()
    {
        int port = 12345;
        IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        IPAddress serverIP = ipHostInfo.AddressList[0];

        IPEndPoint serverEndPoint = new IPEndPoint(serverIP, port);

        // Create a TcpListener.
        TcpListener listener = new TcpListener(serverEndPoint);
        listener.Start();
        Console.WriteLine("Server listening on {0}:{1}", serverIP, port);

        try
        {
            while (isRunning)
            {
                if (listener.Pending())
                {
                    // Accept a client connection.
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected: {0}", client.Client.RemoteEndPoint);
                    connectionsCount++;

                    // Handle the client connection asynchronously.
                    _ = HandleClientAsync(client, listener);
                }
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: {0}", ex);
        }
        finally
        {
            // Stop listening for new client connections.
            listener.Stop();
            Console.WriteLine("Exited");
        }
    }

    static async Task HandleClientAsync(TcpClient client, TcpListener server)
    {
        string status = "ready";
        try
        {
            // Get a stream object for reading and writing.
            using NetworkStream stream = client.GetStream();
            

            while (true)
            {
                Stopwatch stopwatch = new();
                // Read the incoming data.
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received from client: {0}", message);
                stopwatch.Start();


                string response = "";
                switch (message)
                {
                    case "ready\r\n":
                        if (status == "ready") response = "The server is already working";
                        else
                        {
                            status = "ready";
                            response = "The server is ready";
                        }
                        break;
                    case "pause\r\n":
                        if (status == "pause") response = "Server is already paused";
                        else
                        {
                            status = "pause";
                            response = "The server is paused";
                        }
                        break;
                    case "stop\r\n":
                        if (status == "pause") response = "Server is paused. Send 'ready' to start.";
                        else
                        {
                            status = "stop";
                            response = "The server has been stopped by the client";
                        }
                        break;
                    default:
                        if (status == "pause") response = "Server is paused. Send 'ready' to start.";
                        else response = "There is no such command";
                        break;

                }

                if (message == "exit\r\n") return;
                // Send a response back to the client.
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("Sent to client: {0}", response);
                stopwatch.Stop();
                Console.WriteLine($"Время обработки запроса: {stopwatch.Elapsed}.\n");
                if (message == "stop\r\n" && status != "pause") return;     
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error handling client: {0}", ex);
        }
        finally
        {
            // Clean up.
            client.Close();
            if (status == "stop")
            {
                isRunning = false;
                connectionsCount--;
            }
        }
    }
}
