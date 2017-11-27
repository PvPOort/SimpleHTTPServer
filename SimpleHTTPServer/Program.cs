using System;

namespace SimpleHTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int Port = args != null ? (args.Length > 0 ? Int32.Parse(args[0]) : 11111) : 11111;

            HttpWrapper Wrapper = new HttpWrapper(Port);

            Wrapper.Start();

            Console.ReadKey();
        }
    }
}
