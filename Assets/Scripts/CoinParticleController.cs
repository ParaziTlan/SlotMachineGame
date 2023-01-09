using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CoinParticleController : MonoBehaviour
{
    private ParticleSystem coinParticleSystem;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.Burst burstOfEmission;

    private void Awake()
    {
        coinParticleSystem = GetComponent<ParticleSystem>();
        emissionModule = coinParticleSystem.emission;
        burstOfEmission = emissionModule.GetBurst(0);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnSpinFinished += OnSpinFinished;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnSpinFinished -= OnSpinFinished;
    }

    private void OnSpinFinished()
    {
        Result result = SaveLoadManager.GetCachedProgressData.lastResult;
        int coinAmount = Extensions.GetCoinOfResult(result);

        if (coinAmount > 0)
        {
            burstOfEmission.count = coinAmount;
            emissionModule.SetBurst(0, burstOfEmission);
            coinParticleSystem.Play();
        }
    }

}
