using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class result_creator_test
{
    [Test]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(13)]
    public void is_order_of_periods_correct(int dividingToHowManyPeriods)
    {
        Period[] periods = ResultsCreator.GetPeriods(100, dividingToHowManyPeriods);

        Assert.AreEqual(dividingToHowManyPeriods, periods.Length);
        Assert.AreEqual(0, periods.First().startIndex);
        Assert.AreEqual(100, periods.Last().endIndex);

        int periodIndex = 0;
        for (int i = 0; i < 100; i++)
        {
            if (i == periods[periodIndex].endIndex) periodIndex++;

            Assert.GreaterOrEqual(i, periods[periodIndex].startIndex);
            Assert.Less(i, periods[periodIndex].endIndex);
        }
    }

    [Test]
    public void is_available_indices_calculating_correctly()
    {
        Result[] results = new Result[10];
        int startIndex = 0;
        int endIndex = 5;

        List<int> availableIndices = ResultsCreator.GetAvailableIndices(results, startIndex, endIndex);

        availableIndices.ForEach(i =>
        {
            Assert.GreaterOrEqual(i, startIndex);
            Assert.Less(i, endIndex);
        });

        Assert.AreEqual(5, availableIndices.Count);
        Assert.Contains(2, availableIndices);
        Assert.Contains(3, availableIndices);


        results[2] = new Result();
        results[3] = new Result();

        availableIndices = ResultsCreator.GetAvailableIndices(results, startIndex, endIndex);

        Assert.AreEqual(3, availableIndices.Count);
        Assert.IsFalse(availableIndices.Contains(2), "list should not contain 2");
        Assert.IsFalse(availableIndices.Contains(3), "list should not contain 3");
    }

    [Test]
    public void is_calculated_results_matching_with_hundred_percentages_of_scriptable()
    {
        List<ResultWithOdd> resultsWithOdds = TestContainer.ResultOddsScriptable.resultsWithOddsList;

        List<Result> calculatedResults = ResultsCreator.CreateResults(resultsWithOdds).ToList();
        List<Counter> counters = new List<Counter>();
        resultsWithOdds.ForEach(i => counters.Add(new Counter(i)));

        calculatedResults.ForEach(result => counters.FirstOrDefault(counter => counter.result == result).counter++);

        resultsWithOdds.ForEach(resultWithOdd =>
            Assert.AreEqual(resultWithOdd.hundredPercentage, counters.FirstOrDefault(counter =>
                counter.result == resultWithOdd).counter));

    }

    [Test]
    public void is_calculated_results_matching_with_periods()
    {
        for (int k = 0; k < 1000; k++)
        {
            List<ResultWithOdd> resultsWithOdds = TestContainer.ResultOddsScriptable.resultsWithOddsList;
            List<Result> calculatedResults = ResultsCreator.CreateResults(resultsWithOdds, 100).ToList();

            foreach (ResultWithOdd currentResultWithOdd in resultsWithOdds)
            {
                Period[] periods = ResultsCreator.GetPeriods(100, currentResultWithOdd.hundredPercentage);
                foreach (Period currentPeriod in periods)
                {
                    int counter = 0;
                    for (int i = currentPeriod.startIndex; i < currentPeriod.endIndex; i++)
                    {
                        if (calculatedResults[i] == currentResultWithOdd)
                        {
                            counter++;
                        }
                    }
                    Assert.AreEqual(1, counter);
                }
            }
        }
    }

    public class Counter
    {
        public readonly Result result;
        public int counter;

        public Counter(Result result)
        {
            this.result = result;
        }
    }



}
