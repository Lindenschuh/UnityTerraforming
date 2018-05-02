using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseBrush : Brush
{
    public override float[,] CalculateBrushUp(float[,] currentBrushValues)
    {
        return CSHandler.CalculateWihtShader(currentBrushValues, Value, EasingFunctions.Cubic);
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        return CSHandler.CalculateWihtShader(currentBrushValues, -Value, EasingFunctions.Cubic);
    }
}