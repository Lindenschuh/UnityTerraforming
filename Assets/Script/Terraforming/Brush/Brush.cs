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

    protected virtual void Start()
    {
        MakeIndicator();
    }

    private void FixedUpdate()
    {
        HoverIndicator.SetActive(false);
        if (Terrain.CalculateInpactPoint(Input.mousePosition, out lastRelativePoint, out lastMouseImpactPoint))
        {
            if (IsAreaFree(lastRelativePoint))
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
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            ChangebrushSize((int)Mathf.Max(BrushWidth + 5f, 0), (int)Mathf.Max(BrushHeight + 5f, 0));
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            ChangebrushSize((int)Mathf.Max(BrushWidth - 5f, 0), (int)Mathf.Max(BrushHeight - 5f, 0));
        }
    }

    private bool IsAreaFree(Vector3 destination)
    {
        RaycastHit[] hits;
        hits = Physics.BoxCastAll(destination, new Vector3(BrushWidth / 2, 5f, BrushHeight / 2), Vector3.one);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == Terrain.gameObject)
                continue;

            if (hit.rigidbody == null)
                return false;

            if (hit.rigidbody.isKinematic)
                return false;
        }

        return true;
    }

    public void ChangebrushSize(int width, int height)
    {
        BrushHeight = height;
        BrushWidth = width;
        MakeIndicator();
    }

    protected virtual void MakeIndicator()
    {
        if (gameObject == null)
        {
            HoverIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            HoverIndicator.GetComponent<Renderer>().material = IndicatorMaterial;
            HoverIndicator.SetActive(false);
        }
        HoverIndicator.transform.localScale = new Vector3(BrushWidth, .5f, BrushHeight);
    }
}