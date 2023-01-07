using UnityEngine;

[ExecuteInEditMode]
public class ShaderTester : MonoBehaviour
{
    [SerializeField] private Renderer rend;

    [SerializeField]
    [Range(0, 8)]
    private int imageIndex = 1;

    [SerializeField]
    [Range(0f, 1f)]
    private float blurNormalized = 0f;

    [SerializeField]
    private bool isSlotMachineEnabled = false;

    void OnEnable()
    {
        UpdateMaterialProperties();
    }

    void OnDisable()
    {
        UpdateMaterialProperties();
    }

    void Update()
    {
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (rend == null) rend = GetComponent<Renderer>();

        if (rend != null)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_ImageIndex", imageIndex);
            mpb.SetFloat("_BlurSize", blurNormalized);
            mpb.SetFloat("_SlotMachineEnabled", isSlotMachineEnabled ? 1 : 0);
            rend.SetPropertyBlock(mpb);
        }
    }
}
