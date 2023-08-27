using BenchmarkDotNet.Running;

namespace BenchmarkForexPublisher
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