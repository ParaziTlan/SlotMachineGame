using NUnit.Framework;


public class result_odds_scriptable_test
{
    [Test]
    public void result_odds_name_is_correct_and_can_be_loaded()
    {
        ResultOddsScriptable resultOdds = TestContainer.ResultOddsScriptable;
    }

    [Test]
    public void sum_of_odds_equals_to_100()
    {
        ResultOddsScriptable resultOdds = TestContainer.ResultOddsScriptable;

        int sum = 0;
        resultOdds.resultsWithOddsList.ForEach(i => sum += i.hundredPercentage);

        Assert.AreEqual(100, sum);
    }
}

