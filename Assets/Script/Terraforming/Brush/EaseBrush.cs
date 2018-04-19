using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseBrush : Brush
{
    public EaseFunctions Ease;
    private Func<float, float, float, float> EasingFunction;

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
                float distance = ((x - midX) * (x - midX) + (y - midY) * (y - midY));
                if (distance <= radius * radius)
                {
                    ret[y, x] = EasingFunction(ret[y, x], ret[y, x] + Value * sizeModificator, ((radius * radius) - distance) / (radius * radius));
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
                float distance = ((x - midX) * (x - midX) + (y - midY) * (y - midY));
                if (distance <= radius * radius)
                {
                    ret[y, x] = EasingFunction(ret[y, x], ret[y, x] - Value * sizeModificator, ((radius * radius) - distance) / (radius * radius));
                }
            }
        }

        return ret;
    }

    protected override bool IsAreaFree(Vector3 destination)
    {
        Collider[] colls = Physics.OverlapSphere(destination, BrushWidth / 2);

        foreach (Collider col in colls)
        {
            if (col.gameObject == Terrain.gameObject)
                continue;

            if (col.GetComponent<Rigidbody>() == null)
                return false;

            if (col.GetComponent<Rigidbody>().isKinematic)
                return false;
        }

        return true;
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
        EasingFunction = TerraMath.GetEaseFunction(Ease);
    }
}