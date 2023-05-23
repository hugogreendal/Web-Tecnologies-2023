
using System.Net.Sockets;
//using System.Net;
using System.Text;
//using System.Net.Http;
using System.Net.Security;
using Renci.SshNet;
#nullable disable

namespace web_tech_lab1
{
	public class WebServerConnection
	{
        static void TelnetConnection()
        {
            //alternative website to connect to
            //TelnetConnection Tcon = new TelnetConnection("india.colorado.edu", 13);
            using TelnetConnection Tcon = new TelnetConnection("bbs.vslib.cz", 23);

            string t = Tcon.Login("root", "rootpassword", 130);
            Console.Write(t);
            string Prmpt = t.TrimEnd();
            Prmpt = t.Substring(Prmpt.Length - 1, 1);

            Prmpt = "";
            while (Tcon.IsConnected && Prmpt.Trim() != "exit")
            {
                Console.Write(Tcon.Read());
                Prmpt = Console.ReadLine();
                Tcon.WriteLine(Prmpt);
                Console.Write(Tcon.Read());
            }
            Console.WriteLine("\nDisconnected");
        }

        static void SSHConnection(string host = "underhound.eu",
                                  int port = 22,
                                  string username = "terminal",
                                  string password = "terminal")
        {
            using (var client = new SshClient(host, port, username, password))
            {
                try
                {
                    client.Connect();
                    Console.WriteLine("Connected to the SSH server.");
                    client.RunCommand("ls -l");
                    
                    var command = client.CreateCommand("ssh-keygen");
                    command.Execute();
                    var result = command.Result;
                    Console.WriteLine($"{command.Error}");
                    if (result != "\u001b[2J")
                    {
                        Console.WriteLine(result);
                    }
                    
                    Console.WriteLine("Command executed.");
                    client.Disconnect();
                    Console.WriteLine("Disconnected from the SSH server.");
                }
                catch (Renci.SshNet.Common.SshOperationTimeoutException)
                {
                    Console.Out.Write("Connection failed to establish within 30000 milliseconds");
                }
                catch (Exception e)
                {
                    Console.Out.Write(e);
                }
            }
        }

        static void HTTPConnection(string serverName, int port)
        {
            try
            {
                using TcpClient client = new (serverName, port);
                using NetworkStream netStream = client.GetStream();
                using SslStream sslStream = new SslStream(netStream);

            
                if (port == 443)
                    sslStream.AuthenticateAsClient(serverName);

                byte[] sendBuffer = Encoding.UTF8.GetBytes($"GET / HTTP/1.1\r\nHost: {serverName}\r\nConnection: Close\r\n\r\n");
                int bytesReceived;
                byte[] receiveBuffer = new byte[2048];

                if (port == 443)
                    sslStream.Write(sendBuffer);
                else
                    netStream.Write(sendBuffer);
                
                
                if (port == 443)
                    bytesReceived = sslStream.Read(receiveBuffer);
                else
                    bytesReceived = netStream.Read(receiveBuffer);

                
                File.WriteAllBytes("file.txt", receiveBuffer);

                string data = Encoding.UTF8.GetString(receiveBuffer.AsSpan(0, bytesReceived));

                Console.WriteLine($"This is what the peer sent to you:\n{data}");
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("Timeout error occurred.");
                }
                else if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Console.WriteLine("Connection reset by peer");
                }
                else if (se.SocketErrorCode == SocketError.HostNotFound)
                {
                    Console.WriteLine("No such host");
                }
                else if (se.SocketErrorCode == SocketError.HostUnreachable)
                {
                    Console.WriteLine("No such port");
                }
                else
                {
                    Console.WriteLine("Socket error occurred: " + se.SocketErrorCode);
                }
            }
            catch (Exception e)
            {
                Console.Out.Write(e);
            }
            
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("SSH:\n\n");
            SSHConnection("underhound.eu", 23, "terminal", "terminal");

            TelnetConnection();

            Console.WriteLine("\n\nHTTP:\n\n");
            HTTPConnection("example.com", 80);

            Console.WriteLine("\n\nHTTPS:\n\n");
            HTTPConnection("example.com", 443);
        }
    }
}

