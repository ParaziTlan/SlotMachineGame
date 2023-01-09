public class PeriodWithResult
{
    public readonly int startIndex; // Inclusive
    public readonly int endIndex; // Exclusive
    public readonly Result result;

    public PeriodWithResult(Period period, Result result)
    {
        startIndex = period.startIndex;
        endIndex = period.endIndex;
        this.result = result;
    }
}