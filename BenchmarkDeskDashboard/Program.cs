using BenchmarkDotNet.Running;

namespace BenchmarkDeskDashboard
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var summary = BenchmarkRunner.Run<DataProducerForex>();
        }
    }
}