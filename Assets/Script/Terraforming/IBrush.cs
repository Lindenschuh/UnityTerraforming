using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBrush
{
    float[,] CalculateBrushValues(float[,] currentBrushValues);
}