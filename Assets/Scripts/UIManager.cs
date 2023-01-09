using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static Action OnPlayerTappedSpinButton;

    [SerializeField]
    private Text coinText; // I Do not wanted to import TMP essentials etc. to this tiny case project. (I know TMPro is better than UI.Text)

    private const string CointStr = "Coin : ";

    private void OnEnable()
    {
        GameManager.Instance.OnSpinFinished += OnSpinFinished;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnSpinFinished -= OnSpinFinished;
    }

    private void Start()
    {
        coinText.text = CointStr + SaveLoadManager.GetCachedProgressData.coinAmount.ToString();
    }

    public void OnSpinButtonClicked() // Button CallBack
    {
        if (GameManager.Instance.IsWaitingForInput)
        {
            OnPlayerTappedSpinButton?.Invoke();
        }
    }

    private void OnSpinFinished()
    {
        coinText.text = CointStr + SaveLoadManager.GetCachedProgressData.coinAmount.ToString();
    }
}