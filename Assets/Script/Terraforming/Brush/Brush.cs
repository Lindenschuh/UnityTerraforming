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

    protected GameObject HoverIndicator;
    public ShaderManager CSHandler;

    public abstract float[,] CalculateBrushUp(float[,] currentBrushValues);

    public abstract float[,] CalculateBrushDown(float[,] currentBrushValues);

    protected virtual void Start()
    {
        MakeIndicator();
        ChangebrushSize(BrushWidth, BrushHeight);
    }

    protected void PlaceIndicator(bool isFree, Vector3 destination)
    {
        HoverIndicator.SetActive(true);
        HoverIndicator.transform.position = destination;
        HoverIndicator.GetComponent<Renderer>().material = isFree ? GoodIndicator : BadIndicator;
    }

    public void ChangebrushSize(int width, int height)
    {
        BrushHeight = (int)Mathf.Clamp(height, 5, MaxBrushValue);
        BrushWidth = (int)Mathf.Clamp(width, 5, MaxBrushValue);
        float percentage = (MaxBrushValue + 10) * 2 - (BrushWidth + BrushHeight);
        sizeModificator = percentage / ((MaxBrushValue + 10) * 2);
        MakeIndicator();
    }

    protected virtual void MakeIndicator()
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