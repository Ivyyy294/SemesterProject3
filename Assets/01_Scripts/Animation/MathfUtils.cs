using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfUtils
{
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

    public static float RemapClamped(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float remapped = Remap(value, fromMin, fromMax, toMin, toMax);
        return Mathf.Clamp(remapped, Mathf.Min(toMin, toMax), Mathf.Max(toMin, toMax));
    }
}
