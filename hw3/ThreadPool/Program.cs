using ThreadPool;

MyThreadPool? threadPool = new(3);

var myFunc = () => 2 - 123;
var myContinuation = (int x) =>
{
    Console.WriteLine($"Result = {x}");
    return x;
};

var firstTask = threadPool.Submit(myFunc);
Console.WriteLine(firstTask.Result);
firstTask.ContinueWith(myContinuation);

threadPool.Shutdown();