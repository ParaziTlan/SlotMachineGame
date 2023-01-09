using System;

[Serializable]
public class Result
{
    public SlotObjectTypes column1;
    public SlotObjectTypes column2;
    public SlotObjectTypes column3;

    public SlotObjectTypes GetSlotPieceTypeWithIndex(int index)
    {
        if (index == 0) return column1;
        if (index == 1) return column2;
        if (index == 2) return column3;
        else
        {
            UnityEngine.Debug.LogError("Not have implemented column for " + index);
            return SlotObjectTypes.Bonus;// NULL .. do not wanted to throw any exception
        }
    }
}

[Serializable]
public class ResultWithOdd : Result
{
    public int hundredPercentage;
}
