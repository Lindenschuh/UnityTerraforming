using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRoundBrush : Brush
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
                float distance = ((x - midX) * (x - midX) + (y - midY) * (y - midY));
                float quaterRadius2 = (radius * radius) / 4f;

                if (distance <= quaterRadius2)
                {
                    ret[y, x] += Value;
                }
                else if (distance <= (radius * radius))
                {
                    float smoothing = (quaterRadius2 * 3f - (distance - quaterRadius2)) / (quaterRadius2 * 3f);
                    ret[y, x] += Value * smoothing * sizeModificator;
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
                float quaterRadius2 = (radius * radius) / 4f;

                if (distance <= quaterRadius2)
                {
                    ret[y, x] -= Value;
                }
                else if (distance <= (radius * radius))
                {
                    float smoothing = (quaterRadius2 * 3f - (distance - quaterRadius2)) / (quaterRadius2 * 3f);
                    ret[y, x] -= Value * smoothing * sizeModificator;
                }
            }
        }

        return ret;
    }

    public override bool IsAreaFree(Vector3 destination)
    {
        Collider[] colls = Physics.OverlapSphere(destination, BrushWidth / 2);

        foreach (Collider hit in colls)
        {
            if (hit.GetComponent<Terrain>() != null || hit.gameObject == HoverIndicator)
                continue;

            if (hit.GetComponent<Rigidbody>() == null)
            {
                PlaceIndicator(false, destination);
                return false;
            }

            if (hit.GetComponent<Rigidbody>().isKinematic)
            {
                PlaceIndicator(false, destination);
                return false;
            }
        }
        PlaceIndicator(true, destination);
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
    }
}