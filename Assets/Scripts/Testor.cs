using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testor : MonoBehaviour
{

    public Result[] results;
    public ResultOddsScriptable resultOddsScriptable;

    void Start()
    {
        results = ResultsCreator.CreateResults(resultOddsScriptable.resultsWithOddsList);
    }


}
