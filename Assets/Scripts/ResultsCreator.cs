using UnityEngine;
using System.Collections.Generic;

public class ResultsCreator
{
    public static Result[] CreateResults(List<ResultWithOdd> resultWithOdds, int resultAmount = 100)
    {
        Result[] calculatedResults = new Result[resultAmount]; // Create calculatedResults array

        //Create And Fill openPeriodsWithResults List
        List<PeriodWithResult> openPeriodWithResults = new List<PeriodWithResult>();
        foreach (ResultWithOdd currentResultWithOdd in resultWithOdds)
        {
            Period[] periodsOfCurrentResults = ResultsCreator.GetPeriods(resultAmount, currentResultWithOdd.hundredPercentage);
            foreach (Period currentPeriod in periodsOfCurrentResults)
            {
                openPeriodWithResults.Add(new PeriodWithResult(currentPeriod, currentResultWithOdd));
            }
        }

        //We filling calculatedResults starting from 0 to end one by one
        for (int i = 0; i < calculatedResults.Length; i++)
        {
            //Getting every available(open) periodWithResults for this individual index;
            List<PeriodWithResult> openedPeriodsWithResultOnCurrentIndexList = ResultsCreator.GetOpenedPeriodWithResultsThatAreInTheRangeOfIndex(i, openPeriodWithResults);

            //Getting every available(open) periodWithResults which are ending at the same index
            List<PeriodWithResult> openedPeriodsWithResultsEndingAtSamePlaceList = ResultsCreator.GetPeriodsWithResultsThatAreEndingAtSameIndex(openedPeriodsWithResultOnCurrentIndexList);

            //Checks and returns first available(open) periodWithResult is ending this individual index. If any periodWithResult is not ending at this individual index returns null;
            PeriodWithResult periodWithResultEndingOnThisIndex = ResultsCreator.GetPeriodWithResultEndingOnIndex(openedPeriodsWithResultOnCurrentIndexList, i);

            // Sometimes openedPeriodsWithResultOnCurrentIndexList list will be empty, and throws outOfRangeException. So it is a funky-fix for this situation that i am not proud of :(
            // .. I do not have enough time for now // TODO
            if (openedPeriodsWithResultOnCurrentIndexList.Count == 0)
            {
                return CreateResults(resultWithOdds, resultAmount);
            }

            if (periodWithResultEndingOnThisIndex != null) //It is the last chance for periodWithResultEndingOnThisIndex to choose as result
            {
                calculatedResults[i] = periodWithResultEndingOnThisIndex.result; // we are choosing it as a result
                openPeriodWithResults.Remove(periodWithResultEndingOnThisIndex); // removing from available(open) periodWithResults List.
            }
            else if (openedPeriodsWithResultsEndingAtSamePlaceList.Count > 1) // we have x amount of openedPeriods With the same endIndex  .. x (is)= openedPeriodsWithResultsEndingAtSamePlaceList.Count
            {
                if (openedPeriodsWithResultsEndingAtSamePlaceList[0].endIndex - i <= openedPeriodsWithResultsEndingAtSamePlaceList.Count) //and current index is less or equal to x .. x (is)= openedPeriodsWithResultsEndingAtSamePlaceList.Count
                {
                    PeriodWithResult choosedPeriodWithResult = openedPeriodsWithResultsEndingAtSamePlaceList[UnityEngine.Random.Range(0, openedPeriodsWithResultsEndingAtSamePlaceList.Count)]; // Chosing random one in openedPeriodsWithResultsEndingAtSamePlaceList 
                    calculatedResults[i] = choosedPeriodWithResult.result;  // we are choosing it as a result
                    openPeriodWithResults.Remove(choosedPeriodWithResult);  // removing from available(open) periodWithResults List.
                }
                else // we have x amount of opened periods with the same endIndex .. But currentIndex is far away more than x .. x (is)= openedPeriodsWithResultsEndingAtSamePlaceList.Count
                {
                    PeriodWithResult choosedPeriodWithResult = openedPeriodsWithResultOnCurrentIndexList[UnityEngine.Random.Range(0, openedPeriodsWithResultOnCurrentIndexList.Count)]; //Chosing random one in openedPeriodsWithResultOnCurrentIndexList
                    calculatedResults[i] = choosedPeriodWithResult.result;  // we are choosing it as a result
                    openPeriodWithResults.Remove(choosedPeriodWithResult);  // removing from available(open) periodWithResults List.
                }
            }
            else //There is no unique procedure is active. So we are chosing random result from openedPeriodsWithResultOnCurrentIndexList;
            {
                PeriodWithResult choosedPeriodWithResult = openedPeriodsWithResultOnCurrentIndexList[UnityEngine.Random.Range(0, openedPeriodsWithResultOnCurrentIndexList.Count)];
                calculatedResults[i] = choosedPeriodWithResult.result;  // we are choosing it as a result
                openPeriodWithResults.Remove(choosedPeriodWithResult);  // removing from available(open) periodWithResults List.
            }
        }

        return calculatedResults;
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

    public static List<PeriodWithResult> GetOpenedPeriodWithResultsThatAreInTheRangeOfIndex(int index, List<PeriodWithResult> allPeriodsWithResult)
    {
        List<PeriodWithResult> openedResPerOfIndex = new List<PeriodWithResult>();

        for (int i = 0; i < allPeriodsWithResult.Count; i++)
        {
            if (allPeriodsWithResult[i].startIndex <= index & allPeriodsWithResult[i].endIndex > index)
            {
                openedResPerOfIndex.Add(allPeriodsWithResult[i]);
            }
        }

        return openedResPerOfIndex;
    }

    public static List<PeriodWithResult> GetPeriodsWithResultsThatAreEndingAtSameIndex(List<PeriodWithResult> periodsWithResultsToLookAt)
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

    public static PeriodWithResult GetPeriodWithResultEndingOnIndex(List<PeriodWithResult> periodsWithResultsToLookAt, int index)
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

}
