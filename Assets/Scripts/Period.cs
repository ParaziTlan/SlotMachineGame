public class Period
{
    public readonly int startIndex; // Inclusive
    public readonly int endIndex; // Exclusive

    public Period(int startIndex, int endIndex)
    {
        this.startIndex = startIndex;
        this.endIndex = endIndex;
    }
}
