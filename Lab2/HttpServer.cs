using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;

public class LocalHttpServer
{
    private readonly HttpListener listener;
    private readonly int port;

    public LocalHttpServer(int port)
    {
        this.port = port;
        this.listener = new HttpListener();
        this.listener.Prefixes.Add($"http://localhost:{port}/");
    }

    public void Run()
    {
        this.listener.Start();
        Console.WriteLine("Server started");

        while (true)
        {
            var context = listener.GetContext();
            Console.WriteLine("Context accepted");
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessContext), context);
        }
    }

    private void ProcessContext(object obj)
    {
        var context = (HttpListenerContext)obj;
        var request = context.Request;
        try
        {
            var headers = request.Headers;
            var body1 = new StreamReader(request.InputStream).ReadToEnd();

            Console.WriteLine($"{request.HttpMethod} {request.Url} HTTP/{request.ProtocolVersion}");
            foreach (string headerName in headers.AllKeys)
            {
                Console.WriteLine($"{headerName}: {headers[headerName]}");
            }
            Console.WriteLine();
            Console.WriteLine(body1);
            
            Thread.Sleep(5000);
            var RequestMethod = request.HttpMethod;
            string RequestFilePath = request.RawUrl;
            string RequestFileName = RequestFilePath.Substring(RequestFilePath.LastIndexOf("/") + 1);
            var RequestProtocol = request.ProtocolVersion;
            var RequestContentType = request.ContentType;
            var RequestAccept = request.AcceptTypes;

            byte[] body;
            var NotFound = ToBytes($"HTTP/{RequestProtocol} 404 Not Found\r\n\r\n");
            var NotImplemented = ToBytes($"HTTP/{RequestProtocol} 501 Not Implemented\r\n\r\n");
            var VersionNotSupported = ToBytes($"HTTP/{RequestProtocol} 505 Version Not Supported\r\n\r\n");

            if (RequestProtocol == new Version(1, 1) || RequestProtocol == new Version(1, 0))
                Console.WriteLine("Version supported");
            else
            {
                SendResponse(context, VersionNotSupported);
                WriteToLog(VersionNotSupported);
            }
            
            if (File.Exists(Path.Combine("resources", RequestFileName)))
            {
                if (RequestAccept != null)
                {
                    if (RequestAccept.Contains("application/xml"))
                        RequestFileName = "example.xml";
                    else if (RequestAccept.Contains("application/json"))
                        RequestFileName = "example.json";
                    else if (RequestAccept.Contains("text/html"))
                        RequestFileName = "example.html";
                }
                
                body = File.ReadAllBytes(Path.Combine("resources", RequestFileName));
            }
            else
            {
                SendResponse(context, NotFound);
                WriteToLog(NotFound);
                return;
            }

            if (RequestMethod == "GET")
            {
                var GetOK = ToBytes($"HTTP/{RequestProtocol} 200 OK\r\n" +
                                     "Content-Type: text/html\r\n" +
                                    $"Content-Length: {body.Length}\r\n\r\n");
                SendResponse(context, GetOK, body);

            }
            else if (RequestMethod == "HEAD")
            {
                var HeadOK = ToBytes($"HTTP/{RequestProtocol} 200 OK\r\n" +
                                      "Content-Type: text/html\r\n" +
                                     $"Content-Length: {body.Length}\r\n\r\n");
                SendResponse(context, HeadOK);
                Console.WriteLine("response sent");
            }
            else
            {
                SendResponse(context, NotImplemented);
                WriteToLog(NotImplemented);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error occured: " + e.ToString());
            SendResponse(context, ToBytes("500 Internal Server Error"));
            WriteToLog(ToBytes("500 Internal Server Error"));
        }
    }

    private byte[] ToBytes(string Response)
    {
        return Encoding.UTF8.GetBytes(Response);
    }

    private void WriteToLog(byte[] Error)
    {
        var log = File.Open("Log.txt", FileMode.Append);
        log.Write(Error);
        log.Write(ToBytes("------------------------------\r\n"));
        log.Close();
    }

    private void SendResponse(HttpListenerContext context, byte[] header, byte[]? body = null)
    {
        context.Response.OutputStream.Write(header, 0, header.Length);
        if (body != null)
            context.Response.OutputStream.Write(body, 0, body.Length);
        context.Response.OutputStream.Flush();
        context.Response.OutputStream.Close();
    }
}
