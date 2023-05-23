using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
#nullable disable

namespace web_tech_lab1
{
    public class TCPClient
    {
        static async Task Main()
        {
            try
            {
                Console.WriteLine("Введите имя сервера: ");
                string serverName = Console.ReadLine();
                Console.WriteLine("Введите порт для подключения: ");
                int port = Convert.ToInt32(Console.ReadLine());

                using TcpClient client = new();
                await client.ConnectAsync(serverName, port);
                using NetworkStream netStream = client.GetStream();

                while (true)
                {
                    

                    // Send some data to the peer.
                    Console.WriteLine("Введите сообщение (stop для выхода):");
                    string responce = Console.ReadLine();
                    byte[] sendBuffer = Encoding.UTF8.GetBytes(responce);
                    int bytesReceived;
                    byte[] receiveBuffer = new byte[2048];
                    
                    await netStream.WriteAsync(sendBuffer);

                    if (responce == "exit") return;

                    // Receive some data from the peer.
                    bytesReceived = await netStream.ReadAsync(receiveBuffer);
                    
                    string data = Encoding.UTF8.GetString(receiveBuffer.AsSpan(0, bytesReceived));
                    Console.WriteLine($"This is what the peer sent to you:\n{data}");
                    if (responce == "stop") return;
                }

            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("Timeout error occurred.");
                }
                else if (se.SocketErrorCode == SocketError.HostNotFound)
                {
                    Console.WriteLine("Such host does not exist");
                }
                else if (se.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    Console.WriteLine("Incorrect port");
                }
                else
                {
                    Console.WriteLine("Socket error occurred: " + se.SocketErrorCode);
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            finally
            {
                Console.Write("You've been disconnected.");
            }

        }
    }
}

