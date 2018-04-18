using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundBrush : Brush
{
    public override float[,] CalculateBrushUp(float[,] currentBrushValues)
    {
        float[,] ret = currentBrushValues;
        int lengthY = currentBrushValues.GetLength(0);
        int lengthX = currentBrushValues.GetLength(1);
        int midX = lengthX / 2;
        int midY = lengthY / 2;
        int radius = BrushWidth / 2;

        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                if (((x - midX) * (x - midX) + (y - midY) * (y - midY)) <= radius * radius)
                {
                    ret[y, x] += Value * sizeModificator;
                }
            }
        }

        return ret;
    }

    public override float[,] CalculateBrushDown(float[,] currentBrushValues)
    {
        float[,] ret = currentBrushValues;
        int lengthY = currentBrushValues.GetLength(0);
        int lengthX = currentBrushValues.GetLength(1);
        int midX = lengthX / 2;
        int midY = lengthY / 2;
        int radius = BrushWidth / 2;
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                if (((x - midX) * (x - midX) + (y - midY) * (y - midY)) <= radius * radius)
                {
                    ret[y, x] -= Value * sizeModificator;
                }
            }
        }

        return ret;
    }

    protected override void MakeIndicator()
    {
        if (HoverIndicator == null)
        {
            HoverIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            HoverIndicator.GetComponent<Renderer>().material = GoodIndicator;
            HoverIndicator.transform.parent = transform;
            HoverIndicator.SetActive(false);
        }
        HoverIndicator.transform.localScale = new Vector3(BrushWidth, .5f, BrushHeight);
    }
}