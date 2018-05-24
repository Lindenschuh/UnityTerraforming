using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public BrushSwitch BS;
    private Material FogOfWarMat;

    private void Start()
    {
        FogOfWarMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    private void Update()
    {
        FogOfWarMat.SetVector("_BoundCenter", new Vector2(BS.BoundCenter.position.x, BS.BoundCenter.position.z));
        FogOfWarMat.SetFloat("_Radius", BS.BoundRadius);
    }
}