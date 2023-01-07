
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class result_creator_test
{
    [Test]
    [TestCase(2)]
    [TestCase(5)]
    [TestCase(6)]
    public void is_order_of_periods_correct(int dividingToHowManyPeriods)
    {
        Period[] periods = ResultsCreator.GetPeriods(100, dividingToHowManyPeriods);
        int periodIndex = 0;

        Assert.AreEqual(dividingToHowManyPeriods, periods.Length);
        Assert.AreEqual(0, periods.First().startIndex);
        Assert.AreEqual(100, periods.Last().endIndex);

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

 
}
