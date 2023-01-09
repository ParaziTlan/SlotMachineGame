using System;
using UnityEngine;



[System.Serializable]
public enum SlotObjectTypes
{
    Jackpot,
    Wild,
    Bonus,
    A,
    Seven
}

public struct SlotPiecePositionAndIndex
{
    public float Position;
    public int Index;

    public SlotPiecePositionAndIndex(float Position, int Index)
    {
        this.Position = Position;
        this.Index = Index;
    }
}

public class SlotPiece
{
    public readonly SlotObjectTypes type;
    public readonly float distanceYBetween2Rows;
    public readonly int allSlotPiecesIndex;

    private readonly float topYPos = 6;
    private readonly float bottomYPos;
    private readonly int slotPieceCount = Enum.GetNames(typeof(SlotObjectTypes)).Length;

    private float posY;

    public SlotPiece(SlotObjectTypes type, float distanceYBetween2Rows, int allSlotPiecesIndex)
    {
        this.type = type;
        this.distanceYBetween2Rows = distanceYBetween2Rows;
        posY = (int)type * -distanceYBetween2Rows;
        this.allSlotPiecesIndex = allSlotPiecesIndex;
        bottomYPos = topYPos - slotPieceCount * distanceYBetween2Rows;
    }

    public SlotPiecePositionAndIndex Tick(float speedInDeltaTime) // -4 olan 6 ya ışınlanmalı 
    {
        posY -= speedInDeltaTime;
        if (posY < bottomYPos) posY += distanceYBetween2Rows * slotPieceCount;

        return new SlotPiecePositionAndIndex(posY, allSlotPiecesIndex);
    }
}

[Serializable]
public class Column
{
    public readonly int columnIndex;
    public readonly SlotPiece[] slotPieces;

    public float speed = 0;

    private float pos = 0;
    public float GetPos => pos;


    private readonly float topYPos = 6;
    private readonly float bottomYPos;
    private readonly float distanceYBetween2Rows;

    public Column(int columnIndex, int slotPiecesCount, float distanceYBetween2Rows)
    {
        this.columnIndex = columnIndex;
        slotPieces = GetSlotPieces(slotPiecesCount, distanceYBetween2Rows, columnIndex);
        this.distanceYBetween2Rows = distanceYBetween2Rows;
        bottomYPos = topYPos - slotPieces.Length * distanceYBetween2Rows;
    }

    private SlotPiece[] GetSlotPieces(int slotPiecesCount, float distanceYBetween2Rows, int columnIndex)  //TODO Move it to the PlayModeExtensions
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
        float speedInTimeDeltaTime = deltaTime * speed;

        pos -= speedInTimeDeltaTime;
        if (pos < bottomYPos) pos += slotPieces.Length * distanceYBetween2Rows;


        SlotPiecePositionAndIndex[] slotPosAndIndicesArray = new SlotPiecePositionAndIndex[slotPieces.Length];
        for (int i = 0; i < slotPieces.Length; i++)
        {
            slotPosAndIndicesArray[i] = slotPieces[i].Tick(speedInTimeDeltaTime);
        }
        return slotPosAndIndicesArray;
    }

}

public class SlotMachineController : MonoBehaviour
{
    [SerializeField]
    private Renderer genericSlotPiecePrefab;

    [SerializeField]
    private float columnStartingPosX = -2.5f, distanceXBetween2Columns = 2.5f, distanceYBetween2Rows = 2f;

    [SerializeField]
    private int columnAmount = 3;

    private Column[] columnsArray;

    private Renderer[] slotPieceRenderers;

    [SerializeField] private float maxSpeed = 10f;

    private float time = 0;
    private bool isSpinning = false;
    private float delayForStartSpinningTime = 0.05f;
    //private float timeForMaxSpeed = 0.5f;
    private float fastStoppingTime = 0.1f;
    private float normalStoppingTime = 1f;
    private float slowStoppingTime = 2.25f;

    private float spinningLoopTime = 1f;

    private float calcAccel1;
    private float calcAccel2;
    private float calcAccel3;

    private float deltaTimeBefore;

    private enum AnimationState
    {
        NoSpin,
        StartingToSpinning,
        WaitingAllOfThemToMaxSpeed,
        SpinningLoopTime,
        Stopping
    }

    private AnimationState currentAnimationState;

    private void ChangeAnimationState(AnimationState toState)
    {
        currentAnimationState = toState;
        time = 0;
    }

    private void Animate()
    {
       
    }


    private void OnEnable()
    {
        GameManager.Instance.OnSpinStarted += OnSpinStarted;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnSpinStarted -= OnSpinStarted;
    }

    private void OnSpinStarted()
    {
        isSpinning = true;
        ChangeAnimationState(AnimationState.StartingToSpinning);

    }

    private void Start()
    {
        CreateColumns();
    }

    private void CreateColumns()
    {
        int slotPieceCount = Enum.GetNames(typeof(SlotObjectTypes)).Length;

        slotPieceRenderers = new Renderer[slotPieceCount * columnAmount];

        columnsArray = new Column[columnAmount];
        for (int i = 0; i < columnAmount; i++)
        {
            Transform columnObject = new GameObject("Column_" + i).transform;
            columnObject.SetParent(transform);
            for (int k = 0; k < slotPieceCount; k++)
            {
                Renderer createdRenderer = Instantiate(genericSlotPiecePrefab);
                createdRenderer.SetSlotImageIndex(k);
                createdRenderer.name = ((SlotObjectTypes)k).ToString();
                createdRenderer.transform.position = new Vector3(columnStartingPosX + i * distanceXBetween2Columns, k * distanceYBetween2Rows, createdRenderer.transform.position.z);
                createdRenderer.transform.SetParent(columnObject);
                slotPieceRenderers[i * slotPieceCount + k] = createdRenderer;
            }

            columnsArray[i] = new Column(i, slotPieceCount, distanceYBetween2Rows);
        }
    }

    private void Update()
    {
        Animate();

        float deltaTime = Time.deltaTime;
        for (int i = 0; i < columnAmount; i++)
        {
            float normalizedSpeed = columnsArray[i].speed.Remap(0, maxSpeed, 0, 1);
            SlotPiecePositionAndIndex[] columnsPiecesPositionAndIndices = columnsArray[i].Tick(deltaTime);
            for (int k = 0; k < columnsPiecesPositionAndIndices.Length; k++)
            {
                slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].transform.position = new Vector3(slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].transform.position.x,
                    columnsPiecesPositionAndIndices[k].Position, slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].transform.position.z);

                slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].UpdateSlotBlurAmount(normalizedSpeed);
            }
        }

        if (Input.GetKey(KeyCode.Y))
        {
            columnsArray[0].speed += Time.deltaTime * maxSpeed;
        }
        if (Input.GetKey(KeyCode.H))
        {
            columnsArray[0].speed -= Time.deltaTime * maxSpeed;
        }

        if (Input.GetKey(KeyCode.U))
        {
            columnsArray[1].speed += Time.deltaTime * maxSpeed;
        }
        if (Input.GetKey(KeyCode.J))
        {
            columnsArray[1].speed -= Time.deltaTime * maxSpeed;
        }

        if (Input.GetKey(KeyCode.I))
        {
            columnsArray[2].speed += Time.deltaTime * maxSpeed;
        }
        if (Input.GetKey(KeyCode.K))
        {
            columnsArray[2].speed -= Time.deltaTime * maxSpeed;
        }

    }

}
