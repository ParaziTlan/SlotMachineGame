using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Result
{
    public SlotObjectTypes column1;
    public SlotObjectTypes column2;
    public SlotObjectTypes column3;
}

[Serializable]
public class ResultWithOdd : Result
{
    public int hundredPercentage;
}

[CreateAssetMenu(fileName = "New Result Odds ScriptableObject", menuName = "New_ResultOddsScriptable")]
public class ResultOddsScriptable : ScriptableObject // I used scriptable object for more designer friendlines
{
    public List<ResultWithOdd> resultsWithOddsList;
}

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

public class ResultsCreator
{
    public static Result[] CreateResults(List<ResultWithOdd> resultWithOdds, int resultAmount = 100)
    {
        Result[] calculatedResults = new Result[resultAmount];

        int falan = 0;

        foreach (ResultWithOdd resultOdd in resultWithOdds)
        {
            Period[] periods = GetPeriods(resultAmount, resultOdd.hundredPercentage);

            foreach (Period currentPeriod in periods)
            {
                Debug.Log(falan + "   " + resultOdd.column1 + " " + resultOdd.column2 + " " + resultOdd.column3);
                Debug.Log(currentPeriod.startIndex + "   " + currentPeriod.endIndex);
                calculatedResults[GetAvailableIndex(calculatedResults, currentPeriod.startIndex, currentPeriod.endIndex)] = resultOdd;
                falan++;
            }
        }
        return calculatedResults;
    }

    public static List<int> GetAvailableIndices(Result[] calculatedResults, int startIndex, int endIndex)
    {
        List<int> availableIndices = new List<int>();

        for (int i = startIndex; i < endIndex; i++)
        {
            if (calculatedResults[i] == null)
            {
                availableIndices.Add(i);
            }
        }

        return availableIndices;
    }

    public static int GetAvailableIndex(Result[] calculatedResults, int startIndex, int endIndex)
    {
        List<int> availableIndices = GetAvailableIndices(calculatedResults, startIndex, endIndex);

        Debug.Log(availableIndices.Count);

        return availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
    }

    public static Period[] GetPeriods(int totalNumber, int dividingToHowManyPeriods)
    {
        Period[] periods = new Period[dividingToHowManyPeriods];
        int index = 0;

        int plusOnePeriodsCount = totalNumber % dividingToHowManyPeriods; //for fracture parts of divided number
        int differenceBetween2Period = Mathf.FloorToInt((float)totalNumber / dividingToHowManyPeriods);

        for (int i = 0; i < dividingToHowManyPeriods; i++)
        {
            Period currentPeriod = new Period(index, index + differenceBetween2Period + (i < plusOnePeriodsCount ? 1 : 0));
            periods[i] = currentPeriod;
            index = currentPeriod.endIndex;
        }

        return periods;
    }

}
