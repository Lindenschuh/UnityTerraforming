using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystral : MonoBehaviour
{
    public float RotationSpeed;
    public Material DefaultMaterial;
    public Material CapturedMaterial;

    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * RotationSpeed));
    }

    public void TooggleCaptured(bool captured) =>
        _renderer.material = (captured) ? CapturedMaterial : DefaultMaterial;
}