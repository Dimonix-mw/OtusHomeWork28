namespace Service
{
    public class CalculatorService
    {
        private int _sumParellelForEach = 0;
        private object _lockParellelForEach = new object();
        private int _sumParellelThreadPool = 0;
        private object _lockParellelThreadPool = new object();
        CountdownEvent _countdownEventThreadPool;
        public int Sum(IEnumerable<int> collection) => collection.Sum();

        public int SumAsParellelForEach(IEnumerable<int> collection)
        {
            _sumParellelForEach = 0;
            Parallel.ForEach(collection, HandlerParellelForEach);
            return _sumParellelForEach;
        }

        public int SumAsParellelTask(IEnumerable<int> collection, int countThread)
        {
            var parts = SplitCollection(collection, countThread);
            var tasks = parts.Select(p => Task.Run(() => Sum(p))).ToArray();
            return tasks.Select(t => t.Result).Sum();
        }

        public int SumAsParellelThreadPool(IEnumerable<int> collection, int countThread)
        {
            _sumParellelThreadPool = 0;
            _countdownEventThreadPool = new CountdownEvent(countThread);

            for (int i = 0; i < countThread; i++)
            {
                ThreadPool.QueueUserWorkItem(HandlerParellelThreadPool, collection.Where((_, ind) => ind % countThread == i).ToArray());
            }
            _countdownEventThreadPool.Wait();
            return _sumParellelThreadPool;
        }

        private void HandlerParellelThreadPool(object obj)
        {
            lock (_lockParellelThreadPool)
            {
                _sumParellelThreadPool += Sum((IEnumerable<int>)obj);
            }
            _countdownEventThreadPool.Signal();
        }
       
        private void HandlerParellelForEach(int item)
        {
            lock (_lockParellelForEach)
            {
                _sumParellelForEach += item;
            }
        }

        private IEnumerable<IEnumerable<int>> SplitCollection(IEnumerable<int> collection, int countThread)
        {
            int i = 0;
            return collection.GroupBy(x => i++ % countThread);
        }

        public int SumLinq(IEnumerable<int> collection) => collection.AsParallel().Sum();
    }
}