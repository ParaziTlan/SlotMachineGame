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

public class ResultsCreator
{
    public static List<PeriodWithResult> GetOpenedPeriodWithResultsInRangeOfIndex(int index, List<PeriodWithResult> allPeriodsWithResult)
    {
        List<PeriodWithResult> openedResPerOfIndex = new List<PeriodWithResult>();

        for (int i = 0; i < allPeriodsWithResult.Count; i++)
        {
            if (allPeriodsWithResult[i].startIndex <= index & allPeriodsWithResult[i].endIndex > index)
            {
                openedResPerOfIndex.Add(allPeriodsWithResult[i]);
                //Debug.Log(allPeriodsWithResult[i].result.column1 + " " + allPeriodsWithResult[i].result.column2 + " " + allPeriodsWithResult[i].result.column3);
            }
        }

        return openedResPerOfIndex;

    }

    public static List<PeriodWithResult> GetPeriodsWithResultsWithTheSameEndIndex(List<PeriodWithResult> periodsWithResultsToLookAt)
    {
        List<PeriodWithResult> sameEndIndicesList = new List<PeriodWithResult>();

        int closestEndIndex = int.MaxValue;

        for (int i = 0; i < periodsWithResultsToLookAt.Count; i++)
        {
            if (periodsWithResultsToLookAt[i].endIndex < closestEndIndex)
            {
                closestEndIndex = periodsWithResultsToLookAt[i].endIndex;
            }
        }

        for (int i = 0; i < periodsWithResultsToLookAt.Count; i++)
        {
            if (periodsWithResultsToLookAt[i].endIndex == closestEndIndex)
            {
                sameEndIndicesList.Add(periodsWithResultsToLookAt[i]);
            }
        }

        return sameEndIndicesList;
    }

    public static PeriodWithResult GetPeriodsWithResultWithTheSameIndex(List<PeriodWithResult> periodsWithResultsToLookAt, int index)
    {
        PeriodWithResult periodWithResultWithEndIndexSameAsIndex = null;

        for (int i = 0; i < periodsWithResultsToLookAt.Count; i++)
        {
            if (periodsWithResultsToLookAt[i].endIndex == index + 1)
            {
                periodWithResultWithEndIndexSameAsIndex = periodsWithResultsToLookAt[i];
                break;
            }
        }

        return periodWithResultWithEndIndexSameAsIndex;
    }

    public static Result[] CreateResults(List<ResultWithOdd> resultWithOdds, int resultAmount = 100)
    {
        //UnityEngine.Random.seed = 29;


        Result[] calculatedResults = new Result[resultAmount];

        List<PeriodWithResult> openPeriodWithResults = new List<PeriodWithResult>();
        for (int i = 0; i < resultWithOdds.Count; i++)
        {
            Period[] periods = ResultsCreator.GetPeriods(resultAmount, resultWithOdds[i].hundredPercentage);
            for (int k = 0; k < periods.Length; k++)
            {
                openPeriodWithResults.Add(new PeriodWithResult(periods[k], resultWithOdds[i]));
            }
        }

        for (int i = 0; i < calculatedResults.Length; i++)
        {
            List<PeriodWithResult> openedPeriodsWithResultOnCurrentIndexList = ResultsCreator.GetOpenedPeriodWithResultsInRangeOfIndex(i, openPeriodWithResults);
            List<PeriodWithResult> openedPeriodsWithResultsWithTheSameEndIndexList = ResultsCreator.GetPeriodsWithResultsWithTheSameEndIndex(openedPeriodsWithResultOnCurrentIndexList);
            PeriodWithResult periodsWithResultWithTheSameIndex = ResultsCreator.GetPeriodsWithResultWithTheSameIndex(openedPeriodsWithResultOnCurrentIndexList, i);

            if (openedPeriodsWithResultOnCurrentIndexList.Count == 0) // Sometimes openedPeriodsWithResultOnCurrentIndexList list will be empty, so it is a trashy-fix :( I do not have enough time for now so : TODO
            {
                return CreateResults(resultWithOdds, resultAmount);
            }


            if (periodsWithResultWithTheSameIndex != null) // we are at the index of some openedPeriodsEndIndex
            {
                calculatedResults[i] = periodsWithResultWithTheSameIndex.result;
                openPeriodWithResults.Remove(periodsWithResultWithTheSameIndex);
            }
            else if (openedPeriodsWithResultsWithTheSameEndIndexList.Count > 1) // we have x amount of openedPeriods With the same endIndex and current index is less or equal to x
            {
                if (openedPeriodsWithResultsWithTheSameEndIndexList[0].endIndex - i <= openedPeriodsWithResultsWithTheSameEndIndexList.Count)
                {
                    PeriodWithResult choosedPeriodWithResult = openedPeriodsWithResultsWithTheSameEndIndexList[UnityEngine.Random.Range(0, openedPeriodsWithResultsWithTheSameEndIndexList.Count)];
                    calculatedResults[i] = choosedPeriodWithResult.result;
                    openPeriodWithResults.Remove(choosedPeriodWithResult);
                }
                else // TOTAL RANDOM
                {
                    //Debug.Log("FALANS"); 
                    // Debug.Log(i + "   " + openedPeriodsWithResultOnCurrentIndexList.Count);
                    PeriodWithResult choosedPeriodWithResult = openedPeriodsWithResultOnCurrentIndexList[UnityEngine.Random.Range(0, openedPeriodsWithResultOnCurrentIndexList.Count)];
                    calculatedResults[i] = choosedPeriodWithResult.result;
                    openPeriodWithResults.Remove(choosedPeriodWithResult);
                }
            }
            else // TOTAL RANDOM
            {
                //Debug.Log(i + "   " + openedPeriodsWithResultOnCurrentIndexList.Count);
                PeriodWithResult choosedPeriodWithResult = openedPeriodsWithResultOnCurrentIndexList[UnityEngine.Random.Range(0, openedPeriodsWithResultOnCurrentIndexList.Count)];
                calculatedResults[i] = choosedPeriodWithResult.result;
                openPeriodWithResults.Remove(choosedPeriodWithResult);
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

    public static Period[] GetPeriods(int totalNumber, int dividingToHowManyPeriods)
    {
        Period[] periods = new Period[dividingToHowManyPeriods];

        int plusOnePeriodsCount = totalNumber % dividingToHowManyPeriods; //for fracture parts of divided number
        int differenceBetween2Period = Mathf.FloorToInt((float)totalNumber / dividingToHowManyPeriods);

        int index = 0;
        for (int i = 0; i < dividingToHowManyPeriods; i++)
        {
            Period currentPeriod = new Period(index, index + differenceBetween2Period + (i < plusOnePeriodsCount ? 1 : 0));
            periods[i] = currentPeriod;
            index = currentPeriod.endIndex;
        }

        return periods;
    }

    //public static int GetRandomIndexInRange(Result[] calculatedResults, int startIndex, int endIndex)
    //{
    //    List<int> availableIndices = GetAvailableIndices(calculatedResults, startIndex, endIndex);

    //    Debug.Log(availableIndices.Count);

    //    return availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
    //}

    //public static int GetAvailableIndex(Result[] calculatedResults, int startIndex, int endIndex, bool sondanGeliyor)
    //{
    //    List<int> availableIndices = GetAvailableIndices(calculatedResults, startIndex, endIndex);

    //    Debug.Log(availableIndices.Count);

    //    return availableIndices[sondanGeliyor ? availableIndices.Count - 1 : 0];
    //}
}
