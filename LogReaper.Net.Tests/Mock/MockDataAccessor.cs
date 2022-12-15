using System.Collections.Concurrent;

namespace LogReaper.Net.Tests.Mock
{
    public class MockDataAccessor<TData>
    {
        private readonly ConcurrentQueue<TData> internalMessagesQueue = new ConcurrentQueue<TData>();

        public void Enqueue(TData message)
        {
            internalMessagesQueue.Enqueue(message);
        }
    }
}
