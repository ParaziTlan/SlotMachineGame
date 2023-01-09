using System;
using UnityEngine;

using Random = UnityEngine.Random;

public static class Extensions
{
    private const string ImageIndexStr = "_ImageIndex";
    private const string BlurSizeStr = "_BlurSize";
    private const string SlotMachineEnabledStr = "_SlotMachineEnabled";

    public static void SetSlotImageIndex(this Renderer rend, int imageIndex)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        mpb.SetFloat(ImageIndexStr, imageIndex);
        mpb.SetFloat(BlurSizeStr, 0);
        mpb.SetFloat(SlotMachineEnabledStr, 0);

        rend.SetPropertyBlock(mpb);
    }

    public static void UpdateSlotBlurAmount(this Renderer rend, float speedNormalized)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);

        mpb.SetFloat(BlurSizeStr, speedNormalized);

        rend.SetPropertyBlock(mpb);
    }



    public static float Remap(this float value, float from1, float to1, float from2, float to2, bool clamped = true)
    {
        return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
    }

    public static void LineerMove(this Transform transformToAnimate, float time, float animationTime, Vector3 fromPos, Vector3 toPos)
    {
        if (time < animationTime)
        {
            fromPos.z = transformToAnimate.position.z;
            toPos.z = transformToAnimate.position.z;
            if (time < animationTime)
            {
                transformToAnimate.position = Vector3.Lerp(fromPos, toPos, time.Remap(0, animationTime, 0, 1));
            }
        }
        else
        {
            transformToAnimate.position = toPos;
        }
    }

}