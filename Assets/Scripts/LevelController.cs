using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnInitializing += OnInitializing;
        GameManager.Instance.OnSpinStarted += OnSpinStarted;
        GameManager.Instance.OnSpinFinished += OnSpinFinished;

        UIManager.OnPlayerTappedSpinButton += OnPlayerTappedSpinButton;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnInitializing -= OnInitializing;
        GameManager.Instance.OnSpinStarted -= OnSpinStarted;
        GameManager.Instance.OnSpinFinished -= OnSpinFinished;

        UIManager.OnPlayerTappedSpinButton -= OnPlayerTappedSpinButton;
    }





    private void OnInitializing()
    {
        GameManager.Instance.StartPlaying();
    }
    private void OnSpinStarted()
    {

    }
    private void OnSpinFinished()
    {

    }

    private void OnPlayerTappedSpinButton()
    {
        GameManager.Instance.StartSpinning();
    }

}
