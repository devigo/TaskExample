using System;
using System.Threading.Tasks;

namespace TaskExample
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();

            Console.WriteLine();
            Console.WriteLine("Please press any key to continue");
            Console.ReadKey();
        }

        private static async Task MainAsync()
        {
            try
            {
                await TestOne();

                Console.WriteLine();

                TestTwo();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine($"MainAsync: an unhandled exception was logged. Exception type {e.GetType()}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"MainAsync: an unhandled exception was logged. Inner exception type {e.InnerException.GetType()}");
                }
            }
        }

        private static async Task TestOne()
        {
            try
            {
                var result = await LongRunnigTask();
                Console.WriteLine($"TestOne: result = {result}");

                var result2 = await LongRunnigTask(true);   // throw TestException
                Console.WriteLine($"TestOne: result2 = {result}");
            }
            catch (TestException e)     // we expected the TestException exception and handled it
            {
                Console.WriteLine($"TestOne: I handled the exception {e.GetType()}");
            }
        }

        private static void TestTwo()
        {
            try
            {
                var result = LongRunnigTask().Result;
                Console.WriteLine($"TestTwo: result = {result}");

                var result2 = LongRunnigTask(true).Result;  // throw AggregateException, TestException was wrapped in an AggregateException
                Console.WriteLine($"TestTwo: result2 = {result}");
            }
            catch (TestException e)     // we expected the TestException exception, but were unable to handle it
            {
                Console.WriteLine($"{nameof(TestOne)}: I handled the exception {e.GetType()}");
            }
        }

        private static async Task<bool> LongRunnigTask(bool shouldThrowException = false)
        {
            await Task.Delay(1000);

            if (shouldThrowException)
            {
                throw new TestException("Something went wrong");
            }

            return true;
        }
    }

    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message)
            : base(message)
        {
        }
    }
}
