using Sample.Client;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SampleAPI api = new SampleAPI() { BaseUri = new Uri("http://localhost:5000") };
            IFoos fooApi = new Foos(api);

            var result = fooApi.GetAllAsync().Result;
        }
    }
}
