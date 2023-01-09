using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameProgressData
{
    public int coinAmount;
    public int slotSpinCount;
    public Result lastResult;
}

public static class SaveLoadManager
{
    private static readonly string resultsSaveStr = "ResultsSaveFile1", gameProgressFileStr = "ProgressSaveFile1";
    private static List<Result> cachedResultsData;
    private static GameProgressData cachedGameProggressData;

    public static void SaveResultsToDisk(List<Result> resultDataToSave)
    {
        FileHandler.SaveToJSON<Result>(resultDataToSave, resultsSaveStr);
        cachedResultsData = resultDataToSave;
    }

    public static void LoadResultsFromDisk()
    {
        cachedResultsData = new List<Result>();
        cachedResultsData = FileHandler.ReadListFromJSON<Result>(resultsSaveStr);

        if (cachedResultsData.Count == 0)
        {
            CreateFirstTimeSaveFile();
        }
    }

    public static List<Result> GetCachedResultsData
    {
        get
        {
            if (cachedResultsData == null)
            {
                LoadResultsFromDisk();
            }
            return cachedResultsData;
        }
    }

    public static void SaveProgressDataToDisk(GameProgressData progressDataToSave)
    {
        FileHandler.SaveToJSON<GameProgressData>(progressDataToSave, gameProgressFileStr);
        cachedGameProggressData = progressDataToSave;
    }

    public static void LoadGameProgressDataFromDisk()
    {
        cachedGameProggressData = FileHandler.ReadFromJSON<GameProgressData>(gameProgressFileStr);

        if (cachedGameProggressData == null) CreateFirstTimeSaveFile();
    }

    public static GameProgressData GetCachedProgressData
    {
        get
        {
            if (cachedGameProggressData == null)
            {
                LoadGameProgressDataFromDisk();
            }
            return cachedGameProggressData;
        }
    }

    public static void CreateFirstTimeSaveFile()
    {
        Debug.Log("Game Starting First Time\nSave File Creating");
        List<Result> newGameSaveFile = ResultsCreator.CreateResults(GameManager.Instance.resultOddScriptable.resultsWithOddsList, 100).ToList(); //Get 100 Results
        SaveResultsToDisk(newGameSaveFile);

        SaveProgressDataToDisk(new GameProgressData()
        {
            slotSpinCount = 0,
            coinAmount = 0
            ,
            lastResult = new Result() { column1 = SlotObjectTypes.Jackpot, column2 = SlotObjectTypes.Jackpot, column3 = SlotObjectTypes.Jackpot }
        });
    }

    public static Result GetNextResult()
    {
        Result result = GetCachedResultsData[0];
        cachedResultsData.RemoveAt(0);

        if (GetCachedResultsData.Count == 0) // Results finished.. Get next 100 Results!
        {
            cachedResultsData = ResultsCreator.CreateResults(GameManager.Instance.resultOddScriptable.resultsWithOddsList, 100).ToList();
        }
        SaveResultsToDisk(cachedResultsData);

        cachedGameProggressData.coinAmount += Extensions.GetCoinOfResult(result); // we change coin data even before even we showing to the player it's result
        cachedGameProggressData.lastResult = result;
        cachedGameProggressData.slotSpinCount++;
        SaveProgressDataToDisk(cachedGameProggressData);

        return result;
    }

}


