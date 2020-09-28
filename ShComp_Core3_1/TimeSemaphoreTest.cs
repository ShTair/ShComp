using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShComp
{
    [TestClass]
    public class TimeSemaphoreTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var sem = new TimeSemaphore(3, TimeSpan.FromSeconds(5));

            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                //await Task.Delay(TimeSpan.FromSeconds(1));

                var ci = i;
                tasks.Add(Task.Run(async () =>
                {
                    Console.WriteLine($"S {ci} {DateTime.Now:HH:mm:ss.ff}");
                    await sem.WaitAsync();
                    Console.WriteLine($"E {ci} {DateTime.Now:HH:mm:ss.ff}");
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
