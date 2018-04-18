using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brush : MonoBehaviour
{
    public float Value;
    public int BrushWidth;
    public int BrushHeight;
    public float MaxBrushValue;
    public float sizeModificator;
    public Material GoodIndicator;
    public Material BadIndicator;
    public TerraManipulation Terrain;

    protected GameObject HoverIndicator;

    private Vector3 lastRelativePoint;
    private Vector2 lastMouseImpactPoint;

    public abstract float[,] CalculateBrushUp(float[,] currentBrushValues);

    public abstract float[,] CalculateBrushDown(float[,] currentBrushValues);

    protected virtual void Start()
    {
        MakeIndicator();
        ChangebrushSize(BrushWidth, BrushHeight);
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
                HoverIndicator.GetComponent<Renderer>().material = GoodIndicator;
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Terrain.ChangeTerrain((int)centeredImpact.x, (int)centeredImpact.y, BrushWidth, BrushHeight, CalculateBrushUp);
                }

                if (Input.GetKey(KeyCode.Mouse1))
                {
                    Terrain.ChangeTerrain((int)centeredImpact.x, (int)centeredImpact.y, BrushWidth, BrushHeight, CalculateBrushDown);
                }
            }
            else
            {
                HoverIndicator.SetActive(true);
                HoverIndicator.transform.position = lastRelativePoint;
                HoverIndicator.GetComponent<Renderer>().material = BadIndicator;
            }
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            ChangebrushSize(BrushWidth + 5, BrushHeight + 5);
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            ChangebrushSize(BrushWidth - 5, BrushHeight - 5);
        }
    }

    private bool IsAreaFree(Vector3 destination)
    {
        RaycastHit[] hits;
        hits = Physics.BoxCastAll(destination, new Vector3(BrushWidth / 2f, 5f, BrushHeight / 2f), Vector3.one);

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
        BrushHeight = (int)Mathf.Clamp(height, 0, MaxBrushValue);
        BrushWidth = (int)Mathf.Clamp(width, 0, MaxBrushValue);
        float percentage = MaxBrushValue * 2 - (BrushWidth + BrushHeight);
        sizeModificator = percentage / (MaxBrushValue * 2);
        MakeIndicator();
    }

    protected virtual void MakeIndicator()
    {
        if (HoverIndicator == null)
        {
            HoverIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            HoverIndicator.GetComponent<Renderer>().material = GoodIndicator;
            HoverIndicator.transform.parent = transform;
            HoverIndicator.SetActive(false);
        }
        HoverIndicator.transform.localScale = new Vector3(BrushWidth, .5f, BrushHeight);
    }
}