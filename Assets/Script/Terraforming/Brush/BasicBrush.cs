using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBrush : Brush
{
    public override float[,] CalculateBrushUp(float[,] currentBrushValues)
    {
        float[,] ret = currentBrushValues;
        for (int y = 0; y < currentBrushValues.GetLength(0); y++)
        {
            for (int x = 0; x < currentBrushValues.GetLength(1); x++)
            {
                ret[y, x] += Value * sizeModificator;
            }
        }

        return ret;
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        float[,] ret = currentBrushValues;
        for (int y = 0; y < currentBrushValues.GetLength(0); y++)
        {
            for (int x = 0; x < currentBrushValues.GetLength(1); x++)
            {
                ret[y, x] -= Value * sizeModificator;
            }
        }

        return ret;
    }
}