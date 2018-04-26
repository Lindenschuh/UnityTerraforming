using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EaseFunctions
{
    CubicIn,
    CubicInOut,
    SineInOut
}

public static class TerraMath
{
    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value + start;
        value -= 2;
        return end * 0.5f * (value * value * value + 2) + start;
    }

    public static float EaseInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
    }

    public static Func<float, float, float, float> GetEaseFunction(EaseFunctions e)
    {
        switch (e)
        {
            case EaseFunctions.CubicIn:
                return EaseInCubic;

            case EaseFunctions.CubicInOut:
                return EaseInOutCubic;

            case EaseFunctions.SineInOut:
                return EaseInOutSine;

            default:
                return Mathf.Lerp;
        }
    }
}