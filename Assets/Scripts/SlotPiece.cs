using System;

public class SlotPiece
{
    public readonly SlotObjectTypes type;
    public readonly float distanceYBetween2Rows;
    public readonly int allSlotPiecesIndex; // It is unique for view parts of program

    private readonly float topYPos = 6;
    private readonly float bottomYPos;
    private readonly int slotPieceCount = Enum.GetNames(typeof(SlotObjectTypes)).Length;
    private readonly float startingPos;

    private float posY;

    public SlotPiece(SlotObjectTypes type, float distanceYBetween2Rows, int allSlotPiecesIndex)
    {
        this.type = type;
        this.distanceYBetween2Rows = distanceYBetween2Rows;
        posY = (int)type * -distanceYBetween2Rows;
        this.allSlotPiecesIndex = allSlotPiecesIndex;
        bottomYPos = topYPos - slotPieceCount * distanceYBetween2Rows;
        startingPos = posY;
    }

    public SlotPiecePositionAndIndex Tick(float columnPos)
    {
        posY = startingPos + columnPos;
        if (posY < bottomYPos) posY += distanceYBetween2Rows * slotPieceCount;
        if (posY > topYPos) posY -= distanceYBetween2Rows * slotPieceCount;

        return new SlotPiecePositionAndIndex(posY, allSlotPiecesIndex);
    }

}
