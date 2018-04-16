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

        float[,] ret = currentBrushValues;

        for (int y = 0; y < currentBrushValues.GetLength(0); y++)
        {
            for (int x = 0; x < currentBrushValues.GetLength(1); x++)
            {
                ret[y, x] = Mathf.Lerp(ret[y, x], mean, Value * sizeModificator);
            }
        }

        return ret;
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        return currentBrushValues;
    }
}