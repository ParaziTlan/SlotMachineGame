using UnityEngine;

[DefaultExecutionOrder(-2)]
public class GameManager : Singleton<GameManager>
{
    public delegate void LevelRelatedActionsDelegate();
    public event LevelRelatedActionsDelegate OnInitializing;
    public event LevelRelatedActionsDelegate OnSpinStarted;
    public event LevelRelatedActionsDelegate OnSpinFinished;

    private enum GameStates
    {
        Initializing,
        WaitingForInput,
        Spinning
    }
    private GameStates state = GameStates.Initializing;
    public bool IsWaitingForInput => state == GameStates.WaitingForInput;

    protected override void Awake()
    {
        base.Awake();
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) Application.targetFrameRate = 60;
    }

    private void Start()
    {
        OnInitializing?.Invoke();
    }

    private void SetState(GameStates toState)
    {
        state = toState;
        if (toState == GameStates.Initializing)
        {
            Debug.LogError("Broken Game-Flow\nCurrent State: " + state.ToString() + "  To State: " + toState.ToString());
        }
        if (toState == GameStates.WaitingForInput)
        {
            OnSpinFinished?.Invoke();
        }
        if (toState == GameStates.Spinning)
        {
            OnSpinStarted?.Invoke();
        }
    }

    public void StartPlaying()
    {
        SetState(GameStates.WaitingForInput);
    }

    public void StartSpinning()
    {
        SetState(GameStates.Spinning);
    }

    public void SpinningFinished()
    {
        SetState(GameStates.WaitingForInput);
    }


}
