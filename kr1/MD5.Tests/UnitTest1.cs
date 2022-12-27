namespace MD5.Tests;

using NUnit.Framework;
using System.Diagnostics;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TimeOfParallelCounterIsLess()
    {
        var time = new Stopwatch();
        var path = "C:/Users/Star/PycharmProjects/copy/spbu_se_site/src/static/currentThesis/reviews/Karaseva_Elizaveta_Olegovna_Autumn_practice_2nd_year_2022_reviewer_review.pdf";
        var md5 = new Counter(path);
        time.Restart();
        var re1 = md5.Start();
        time.Stop();
        var sequentalTime = time.Elapsed.TotalSeconds;

        var mdd5 = new AsyncCounter(path);
        time.Restart();
        var re2 = mdd5.Start();
        time.Stop();
        var parallelTime = time.Elapsed.TotalSeconds;
        Assert.IsTrue(sequentalTime >= parallelTime);
    }

    [Test]
    public void CountersGivesEqualResults()
    {
        var path = "C:/Users/Star/PycharmProjects/copy/spbu_se_site/src/static/currentThesis/reviews";
        var md5 = new Counter(path);
        var re1 = md5.Start();

        var mdd5 = new AsyncCounter(path);
        var re2 = mdd5.Start();
        Assert.AreEqual(re1, re2);
    }
}
