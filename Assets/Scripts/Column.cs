using System;

[Serializable]
public class Column
{
    private enum SlotAnimationState
    {
        NoSpin,
        StartingToSpinning,
        SpinningLoopTime,
        PrepareForStopping,
        Stopping
    }

    private readonly SlotPiece[] slotPieces;
    private readonly float bottomYPos;
    private readonly float distanceYBetween2Rows;
    private readonly float topYPos = 6;
    private readonly float maxSpeed = 40;

    public float GetSpeed => speed;

    private float deccelerationtime = 1f;
    private float decceleration = 0;
    private float speed = 0;
    private float position = 0;
    private float offset;
    private SlotAnimationState currentSlotAnimationState = SlotAnimationState.NoSpin;

    public Column(int columnIndex, int slotPiecesCount, float distanceYBetween2Rows, int startSlotType)
    {
        slotPieces = CreateSlotPieces(slotPiecesCount, distanceYBetween2Rows, columnIndex);
        this.distanceYBetween2Rows = distanceYBetween2Rows;
        bottomYPos = topYPos - slotPieces.Length * distanceYBetween2Rows;
        position = startSlotType * distanceYBetween2Rows;
    }

    private SlotPiece[] CreateSlotPieces(int slotPiecesCount, float distanceYBetween2Rows, int columnIndex)
    {
        SlotPiece[] slotPieces = new SlotPiece[slotPiecesCount];
        for (int i = 0; i < slotPiecesCount; i++)
        {
            slotPieces[i] = new SlotPiece((SlotObjectTypes)i, distanceYBetween2Rows, columnIndex * slotPiecesCount + i);
        }
        return slotPieces;
    }


    public SlotPiecePositionAndIndex[] Tick(float deltaTime)
    {
        //Setting spinning parameters
        if (currentSlotAnimationState == SlotAnimationState.StartingToSpinning)
        {
            if (speed > -maxSpeed)
            {
                speed = -maxSpeed;
                decceleration = 0;
                currentSlotAnimationState = SlotAnimationState.SpinningLoopTime;
            }
        }

        //Kinematic simulation
        speed -= decceleration * deltaTime;
        float speedInTimeDeltaTime = deltaTime * speed;
        position += speedInTimeDeltaTime;
        if (position < bottomYPos)
        {
            position += slotPieces.Length * distanceYBetween2Rows;
        }

        //Setting stopping parameters
        if (currentSlotAnimationState == SlotAnimationState.PrepareForStopping)
        {
            if (position < 0 || position > topYPos)
            {
                currentSlotAnimationState = SlotAnimationState.Stopping;
                speed = -(2 * (position + offset + (position < 0 ? distanceYBetween2Rows * slotPieces.Length : 0))) / deccelerationtime;
                decceleration = speed / deccelerationtime;
            }
        }

        //Checking is it stopped
        if (currentSlotAnimationState == SlotAnimationState.Stopping)
        {
            if (speed > 0)
            {
                speed = 0;
                position = -offset;
                decceleration = 0;
                currentSlotAnimationState = SlotAnimationState.NoSpin;
            }
        }

        //For Returning positions and indices data to view part
        SlotPiecePositionAndIndex[] slotPosAndIndicesArray = new SlotPiecePositionAndIndex[slotPieces.Length];
        for (int i = 0; i < slotPieces.Length; i++)
        {
            slotPosAndIndicesArray[i] = slotPieces[i].Tick(position); //passes calculated position data to slotPiece, and reads its position and index
        }
        return slotPosAndIndicesArray;
    }

    public void StartSpinning()
    {
        currentSlotAnimationState = SlotAnimationState.StartingToSpinning;
        decceleration = maxSpeed * 2f;
    }

    public void Stop(float stoppingTime, int index)
    {
        offset = index * -distanceYBetween2Rows;
        deccelerationtime = stoppingTime;
        currentSlotAnimationState = SlotAnimationState.PrepareForStopping;
    }
}
