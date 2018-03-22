using Microsoft.Extensions.Configuration;
using Email.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Email.Sender.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading...");

			Listener.Start();

			Console.WriteLine("Service started; awaiting items from queue...");
			new AutoResetEvent(false).WaitOne();
		}
	}
}
