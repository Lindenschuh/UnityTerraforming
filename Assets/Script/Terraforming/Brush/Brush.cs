using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brush : MonoBehaviour
{
    public float Value;
    public int BrushWidth;
    public int BrushHeight;
    public Material IndicatorMaterial;
    public TerraManipulation Terrain;

    protected GameObject HoverIndicator;

    private Vector3 lastRelativePoint;
    private Vector2 lastMouseImpactPoint;

    public abstract float[,] CalculateBrushUp(float[,] currentBrushValues);

    public abstract float[,] CalculateBrushDown(float[,] currentBrushValues);

    private void FixedUpdate()
    {
        HoverIndicator.SetActive(false);
        if (Terrain.CalculateInpactPoint(Input.mousePosition, out lastRelativePoint, out lastMouseImpactPoint))
        {
            Vector2 centeredImpact = new Vector2(lastMouseImpactPoint.x - BrushWidth / 2, lastMouseImpactPoint.y - BrushHeight / 2);
            HoverIndicator.SetActive(true);
            HoverIndicator.transform.position = lastRelativePoint;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Terrain.ChangeTerrain((int)centeredImpact.x, (int)centeredImpact.y, BrushWidth, BrushHeight, CalculateBrushUp);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                Terrain.ChangeTerrain((int)centeredImpact.x, (int)centeredImpact.y, BrushWidth, BrushHeight, CalculateBrushDown);
            }
        }
    }
}