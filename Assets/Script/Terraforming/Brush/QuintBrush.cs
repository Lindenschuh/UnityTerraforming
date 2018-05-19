using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuintBrush : Brush
{
    public override float[,] CalculateBrushUp(float[,] currentBrushValues)
    {
        return CSHandler.CalculateWihtShader(currentBrushValues, Value, EasingFunctions.Quint);
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        return CSHandler.CalculateWihtShader(currentBrushValues, -Value, EasingFunctions.Quint);
    }
}