using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GodAbility : MonoBehaviour
{
    public float Radius;
    public Color Indicator_col;
    private GameObject indicator;

    private void Start()
    {
        indicator = transform.parent.GetComponentInChildren<Projector>().gameObject;
        SetUp();
    }

    public abstract void SetUp();

    public abstract void UseAbility(Vector3 Impact);

    public void PlaceIndicator(Vector3 destination)
    {
        indicator.GetComponent<Projector>().material.SetColor("_Color", Indicator_col);
        indicator.transform.position = new Vector3(destination.x,destination.y + 5f,destination.z);
    }
}