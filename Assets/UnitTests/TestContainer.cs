public class TestContainer
{
    private static string scriptableObjectNameForTesting = "ResultsWithOdds";

    public static ResultOddsScriptable ResultOddsScriptable
    {
        get
        {
            if (_resultOddsScriptable == null) _resultOddsScriptable = TestExtensions.LoadScriptableObject<ResultOddsScriptable>(scriptableObjectNameForTesting, nameof(ResultOddsScriptable));
            return _resultOddsScriptable;
        }
    }
    private static ResultOddsScriptable _resultOddsScriptable;

}
