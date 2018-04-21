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

    public abstract float[,] CalculateBrushUp(float[,] currentBrushValues);

    public abstract float[,] CalculateBrushDown(float[,] currentBrushValues);

    protected virtual void Start()
    {
        MakeIndicator();
        ChangebrushSize(BrushWidth, BrushHeight);
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            ChangebrushSize(BrushWidth + 5, BrushHeight + 5);
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            ChangebrushSize(BrushWidth - 5, BrushHeight - 5);
        }
    }

    public virtual bool IsAreaFree(Vector3 destination)
    {
        RaycastHit[] hits;
        hits = Physics.BoxCastAll(destination, new Vector3(BrushWidth / 2f, 5f, BrushHeight / 2f), Vector3.one);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<Terrain>() != null || hit.collider.gameObject == HoverIndicator)
                continue;

            if (hit.rigidbody == null)
            {
                PlaceIndicator(false, destination);
                return false;
            }

            if (hit.rigidbody.isKinematic)
            {
                PlaceIndicator(false, destination);
                return false;
            }
        }
        PlaceIndicator(true, destination);
        return true;
    }

    protected void PlaceIndicator(bool isFree, Vector3 destination)
    {
        HoverIndicator.SetActive(true);
        HoverIndicator.transform.position = destination;
        HoverIndicator.GetComponent<Renderer>().material = isFree ? GoodIndicator : BadIndicator;
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