
namespace web_tech_lab2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			LocalHttpServer HttpServer = new LocalHttpServer(9000);
			HttpServer.Run();
		}
	}
}

