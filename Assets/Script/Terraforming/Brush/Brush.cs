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
    public Color GoodIndicator;
    public Color BadIndicator;
    public ShaderManager CSHandler;

    protected GameObject HoverIndicator;

    private float height = 5f;

    public abstract float[,] CalculateBrushUp(float[,] currentBrushValues);

    public abstract float[,] CalculateBrushDown(float[,] currentBrushValues);

    protected virtual void Start()
    {
        MakeIndicator();
        ChangebrushSize(BrushWidth, BrushHeight);
        HoverIndicator = transform.parent.gameObject.GetComponentInChildren<Projector>().gameObject;
    }

    public void PlaceIndicatorPositive(Vector3 destination)
    {
        HoverIndicator.GetComponent<Projector>().material.SetColor("_Color", GoodIndicator);

        HoverIndicator.SetActive(true);
        HoverIndicator.transform.position = new Vector3(destination.x,destination.y + height,destination.z);
    }

    public void PlaceIndicatorNegative(Vector3 destination)
    {

        HoverIndicator.GetComponent<Projector>().material.SetColor("_Color", BadIndicator);

        HoverIndicator.SetActive(true);
        HoverIndicator.transform.position = new Vector3(destination.x, destination.y + height, destination.z);
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
            Destroy(HoverIndicator.GetComponent<SphereCollider>());
  
            HoverIndicator.transform.parent = transform;
            HoverIndicator.SetActive(false);
        }
        HoverIndicator.transform.localScale = new Vector3(BrushWidth, .5f, BrushHeight);
    }
}