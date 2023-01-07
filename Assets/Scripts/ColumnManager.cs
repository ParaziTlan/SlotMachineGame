using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SlotObjectTypes
{
    Empty = -1, // For both filling algorithm and Unit Tests
    Jackpot,
    Wild,
    Bonus,
    A,
    Seven
}

//[System.Serializable]
public class SlotPiece
{
    public readonly SlotObjectTypes type;
    public SlotPiece(SlotObjectTypes type)
    {
        this.type = type;
    }


}

public class Column
{
    public readonly int columnIndex;

    public Column(int columnIndex)
    {
        this.columnIndex = columnIndex;
        InitializeSlotPieces();

    }

    private void InitializeSlotPieces()
    {

    }



}



public class ColumnManager : MonoBehaviour
{



}
