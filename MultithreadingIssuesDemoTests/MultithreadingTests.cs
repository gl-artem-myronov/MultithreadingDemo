using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace MultithreadingIssuesDemoTests
{
    [TestFixture]
    public class MultithreadingTests
    {
        [Test]
        [Repeat(2)]
        public void AtomicityDemoTest()
        {
            const int incrementCount = 100000;
            const int threadCount = 5;

            int sharedVariable = 0;

            void incrementerFunc()
            {
                for (int i = 0; i < incrementCount; i++)
                {
                    //Interlocked.Increment(ref sharedVariable);
                    sharedVariable += 1;
                }
            }

            var threads = new List<Thread>();
            for (int i = 0; i < threadCount; i++)
            {
                threads.Add(new Thread(incrementerFunc));
            }

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());

            Assert.AreEqual(incrementCount * threadCount, sharedVariable);
        }

        [Test]
        [Repeat(5)]
        public void RaceConditionDemoTest()
        {
            const int itemsCount = 100000;

            var queue = new Queue<int>(itemsCount);

            var appendThread = new Thread(() =>
            {
                for (int i = 0; i < itemsCount; i++)
                {
                    queue.Enqueue(i);
                }
            });

            var readThread = new Thread(() =>
            {
                do
                {
                    queue.Dequeue();
                }
                while (queue.Count > 0);
            });

            appendThread.Start();
            readThread.Start();

            appendThread.Join();
            readThread.Join();

            Assert.AreEqual(0, queue.Count);
        }

    }
}
