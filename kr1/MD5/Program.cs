namespace MD5;

using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var time = new Stopwatch();
        try
        {
            var path = args[0];
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new InvalidDataException("Путь к папке или файлу некорректен!");
            }
            var md5 = new Counter(path);
            time.Restart();
            Console.WriteLine(md5.Start());
            time.Stop();
            var sequentalTime = time.Elapsed.TotalSeconds;

            var mdd5 = new AsyncCounter(path);
            Console.WriteLine();
            time.Restart();
            Console.WriteLine(mdd5.Start());
            time.Stop();
            var parallelTime = time.Elapsed.TotalSeconds;
            Console.WriteLine();
            Console.WriteLine($"Время работы однопоточного варианта: {sequentalTime}, многопоточного: {parallelTime}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
