using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GodAbility : MonoBehaviour
{
    public float Radius;
    public Material Indicator_mat;
    private GameObject indicator;

    private void Start()
    {
        indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(indicator.GetComponent<SphereCollider>());
        indicator.GetComponent<Renderer>().material = Indicator_mat;
        indicator.transform.localScale = new Vector3(Radius * 2, 0.5f, Radius * 2);
        indicator.transform.parent = transform;
        SetUp();
    }

    public abstract void SetUp();

    public abstract void UseAbility(Vector3 Impact);

    public void PlaceIndicator(Vector3 destination)
    {
        indicator.transform.position = destination;
    }
}