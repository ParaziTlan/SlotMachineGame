using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Result Odds ScriptableObject", menuName = "New_ResultOddsScriptable")]
public class ResultOddsScriptable : ScriptableObject // I used scriptable object for more designer friendlines
{
    public List<ResultWithOdd> resultsWithOddsList;
}