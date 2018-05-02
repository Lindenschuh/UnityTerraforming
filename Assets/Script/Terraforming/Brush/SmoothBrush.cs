using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothBrush : Brush
{
    public override float[,] CalculateBrushUp(float[,] currentBrushValues)
    {
        float mean = 0;
        foreach (float v in currentBrushValues)
        {
            mean += v;
        }
        mean /= currentBrushValues.LongLength;

        return CSHandler.CalculateWihtShader(currentBrushValues, mean, EasingFunctions.Mean);
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        return currentBrushValues;
    }
}