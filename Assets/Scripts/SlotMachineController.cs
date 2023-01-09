using System;
using System.Collections;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
    [SerializeField]
    private Renderer genericSlotPiecePrefab;

    [Header("---Slot Machine Settings---")]
    [Space]
    [SerializeField]
    private float columnStartingPosX = -2.5f;
    [SerializeField]
    private float distanceXBetween2Columns = 2.5f, distanceYBetween2Rows = 2f, maxBlurSpeed = 5f, maxBlurAmountAtFullSpeed = 0.4f;

    [Header("---Animation Time Settings---")]
    [Space]
    [SerializeField]
    private float[] startingWaitDelayTimes;
    [SerializeField]
    private float spinningLoopTime = 1f, fastStoppingTime = 0.1f, normalStoppingTime = 1f, slowStoppingTime = 2.25f;


    private Column[] columnsArray;
    private Renderer[] slotPieceRenderers;
    private bool isSpinning = false;

    private static WaitForSeconds waitForAccelerationTime = new WaitForSeconds(0.5f);  // acceleration Time is constant 0.5 second 
    private static WaitForSeconds[] waitForStartingDelayTimeArray;
    private static WaitForSeconds waitForloopTime;
    private static WaitForSeconds waitForFastStoppingTime;
    private static WaitForSeconds waitForNormalStoppingTime;
    private static WaitForSeconds waitForSlowStoppingTime;
    private static WaitForSeconds waitForNextSpinDelay = new WaitForSeconds(0.2f); // I added a little delay for halt players action to make them see coin particles etc...

    private void OnEnable()
    {
        GameManager.Instance.OnSpinStarted += OnSpinStarted;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnSpinStarted -= OnSpinStarted;
    }

    private void Start()
    {
        CreateColumns();

        waitForStartingDelayTimeArray = new WaitForSeconds[startingWaitDelayTimes.Length];
        for (int i = 0; i < startingWaitDelayTimes.Length; i++)
        {
            waitForStartingDelayTimeArray[i] = new WaitForSeconds(startingWaitDelayTimes[i]);
        }
        waitForloopTime = new WaitForSeconds(spinningLoopTime);
        waitForFastStoppingTime = new WaitForSeconds(fastStoppingTime);
        waitForNormalStoppingTime = new WaitForSeconds(normalStoppingTime);
        waitForSlowStoppingTime = new WaitForSeconds(slowStoppingTime);

        GameManager.Instance.StartPlaying();
    }

    private void CreateColumns()
    {
        int slotPieceCount = Enum.GetNames(typeof(SlotObjectTypes)).Length;
        slotPieceRenderers = new Renderer[slotPieceCount * 3];
        columnsArray = new Column[3];
        for (int i = 0; i < 3; i++)
        {
            Transform columnTransform = new GameObject("Column_" + i).transform; // Creating dummy columnTransform just for hierarchy order
            columnTransform.SetParent(transform);
            for (int k = 0; k < slotPieceCount; k++) // Creating slotPieceRenders with prefab just for updating player's view
            {
                Renderer createdRenderer = Instantiate(genericSlotPiecePrefab);
                createdRenderer.SetSlotImageIndex(k);
                createdRenderer.name = ((SlotObjectTypes)k).ToString();
                createdRenderer.transform.position = new Vector3(columnStartingPosX + i * distanceXBetween2Columns, k * distanceYBetween2Rows, createdRenderer.transform.position.z);
                createdRenderer.transform.SetParent(columnTransform);
                slotPieceRenderers[i * slotPieceCount + k] = createdRenderer;
            }

            columnsArray[i] = new Column(i, slotPieceCount, distanceYBetween2Rows, (int)SaveLoadManager.GetCachedProgressData.lastResult.GetSlotPieceTypeWithIndex(i)); //Creating actual column and column creates actual slotpieces
        }
        Tick(0); // Updating player's view with starting parameters
    }

    private void Update()
    {
        if (isSpinning)
        {
            Tick(Time.deltaTime);
        }
    }

    private void Tick(float deltaTime) // Updating columns with deltaTime, columns updates slotPieces, then it returns SlotPiecePositionAndIndex for updating player's view.
    {
        for (int i = 0; i < 3; i++) //For every column
        {
            float normalizedSpeed = Mathf.Abs(columnsArray[i].GetSpeed).Remap(0, maxBlurSpeed, 0, maxBlurAmountAtFullSpeed);
            SlotPiecePositionAndIndex[] columnsPiecesPositionAndIndices = columnsArray[i].Tick(deltaTime); // Column returns its slotPieces' positions and UniqueIndices for view part
            for (int k = 0; k < columnsPiecesPositionAndIndices.Length; k++)
            {
                slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].transform.position = new Vector3(slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].transform.position.x,
                    columnsPiecesPositionAndIndices[k].Position, slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].transform.position.z);

                slotPieceRenderers[columnsPiecesPositionAndIndices[k].Index].UpdateSlotBlurAmount(normalizedSpeed);
            }
        }
    }

    private void OnSpinStarted()
    {
        isSpinning = true;

        StartCoroutine(SpinSequence(SaveLoadManager.GetNextResult()));
    }

    IEnumerator SpinSequence(Result result)
    {
        //Start Spinning with random delays
        for (int i = 0; i < columnsArray.Length; i++)
        {
            if (i != 0)
                yield return waitForStartingDelayTimeArray[UnityEngine.Random.Range(0, waitForStartingDelayTimeArray.Length)];
            columnsArray[i].StartSpinning();
        }
        yield return waitForAccelerationTime;

        //Wait For LoopTime
        yield return waitForloopTime;

        //Set fastStopping animation parameters for default
        WaitForSeconds waitAfterStoppingTime = waitForFastStoppingTime;
        float stoppingTime = fastStoppingTime;

        //Stop Spinning with random delays
        for (int i = 0; i < columnsArray.Length; i++)
        {
            if (i != 0)
                yield return waitForStartingDelayTimeArray[UnityEngine.Random.Range(0, waitForStartingDelayTimeArray.Length)];

            //If we are at last column and first two columns are same .... Setting custom parameters for last column
            if (i == columnsArray.Length - 1 & result.column1 == result.column2)
            {
                bool isSlow = UnityEngine.Random.value > 0.5f;
                stoppingTime = isSlow ? slowStoppingTime : normalStoppingTime;
                waitAfterStoppingTime = isSlow ? waitForSlowStoppingTime : waitForNormalStoppingTime;
            }
            columnsArray[i].Stop(stoppingTime, (int)result.GetSlotPieceTypeWithIndex(i)); // Telling column to stop at which slotPiece at how many seconds later
        }
        yield return waitAfterStoppingTime; //waiting for stopping animation

        yield return waitForNextSpinDelay; //(This is not necessary) waiting littleDelay for if there is particles playing

        isSpinning = false;
        GameManager.Instance.SpinningFinished();
    }

}
