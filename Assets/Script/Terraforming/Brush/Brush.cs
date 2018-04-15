using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brush : MonoBehaviour
{
    public float Value;
    public int BrushWidth;
    public int BrushHeight;

    public GameObject HoverIndicator;
    public Material IndicatorMaterial;

    public abstract float[,] CalculateBrushValues(float[,] currentBrushValues, int modifier);

    private void Start()
    {
    }
}