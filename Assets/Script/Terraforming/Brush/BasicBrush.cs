using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBrush : Brush
{
    private void Start()
    {
        HoverIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        HoverIndicator.transform.localScale = new Vector3(BrushWidth, .5f, BrushHeight);
        HoverIndicator.SetActive(false);
        HoverIndicator.GetComponent<Renderer>().material = IndicatorMaterial;
    }

    public override float[,] CalculateBrushValues(float[,] currentBrushValues, int modifier)
    {
        float applyValue = Value * modifier;
        float[,] ret = currentBrushValues;
        for (int y = 0; y < currentBrushValues.GetLength(0); y++)
        {
            for (int x = 0; x < currentBrushValues.GetLength(1); x++)
            {
                ret[y, x] += applyValue;
            }
        }

        return ret;
    }
}