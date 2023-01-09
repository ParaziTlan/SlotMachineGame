using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Action OnPlayerTappedSpinButton;

    public void OnSpinButtonClicked()
    {
        if (GameManager.Instance.IsWaitingForInput)
        {
            Debug.Log("spin");
            OnPlayerTappedSpinButton?.Invoke();
        }
    }
}
