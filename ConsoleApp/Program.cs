using Service;
using System.Diagnostics;

var sizeArray = 10_000_000;

var arrayInt = Enumerable.Repeat(1, sizeArray);

var calculator = new CalculatorService();

Execute(() => calculator.Sum(arrayInt), "Normal calculation");
Execute(() => calculator.SumAsParellelForEach(arrayInt), "ParallelForEach calculation");
Execute(() => calculator.SumAsParellelThreadPool(arrayInt, 5), "Parallel ThreadPool calculation");
Execute(() => calculator.SumAsParellelTask(arrayInt, 5), "Parallel Task calculation");
Execute(() => calculator.SumLinq(arrayInt), "Linq calculation");

static void Execute(Func<int> func, string nameExecutor)
{
    var sw = new Stopwatch();
    sw.Start();
    var result = func();
    sw.Stop();
    Console.WriteLine($"{nameExecutor} execute time => {sw.ElapsedMilliseconds} ms, result {result}");
} 