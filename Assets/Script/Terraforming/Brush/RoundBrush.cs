using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundBrush : Brush
{
    public override float[,] CalculateBrushUp(float[,] currentBrushValues)
    {
        return CSHandler.CalculateWihtShader(currentBrushValues, Value, EasingFunctions.Lerp);
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        return CSHandler.CalculateWihtShader(currentBrushValues, -Value, EasingFunctions.Lerp);
    }
}