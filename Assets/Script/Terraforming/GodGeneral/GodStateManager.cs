using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodStateManager : Photon.PunBehaviour
{
    public BrushManager BrushMng;
    public AbilityManager AbilityMng;

    public Transform BoundCenter;
    public float BoundRadius;

    private void Start()
    {
        BrushMng.gameObject.SetActive(true);
        AbilityMng.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            photonView.RPC("RPCSwitchMode", PhotonTargets.All);
    }

    public void ChangeBoundRadius(float radius)
    {
        photonView.RPC("RPCChangeRadius", PhotonTargets.All, radius);
    }

    [PunRPC]
    private void RPCSwitchMode()
    {
        BrushMng.gameObject.SetActive(!BrushMng.gameObject.GetActive());
        AbilityMng.gameObject.SetActive(!AbilityMng.gameObject.GetActive());
    }

    [PunRPC]
    private void RPCChangeRadius(float radius)
    {
        BoundRadius = radius;
    }
}