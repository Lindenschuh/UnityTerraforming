using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushSwitch : Photon.PunBehaviour
{
    private List<Brush> AllBrushes = new List<Brush>();
    public List<KeyCode> AllKeys = new List<KeyCode>();
    public Brush CurrentActive;

    public Transform BoundCenter;
    public float BoundRadius;

    // Use this for initialization
    private void Start()
    {
        AllBrushes.AddRange(GetComponentsInChildren<Brush>());
        if (AllBrushes.Count > 0)
        {
            AllBrushes.ForEach(b => b.gameObject.SetActive(false));
            CurrentActive = AllBrushes[0];
            CurrentActive.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            if (Input.GetKeyDown(AllKeys[i]))
                photonView.RPC("RPCSwitchBrush", PhotonTargets.All, i);
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            photonView.RPC("RPCChangeCurrentBrushSize", PhotonTargets.All, CurrentActive.BrushWidth + 5, CurrentActive.BrushHeight + 5);
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            photonView.RPC("RPCChangeCurrentBrushSize", PhotonTargets.All, CurrentActive.BrushWidth - 5, CurrentActive.BrushHeight - 5);
        }
    }

    public void ChangeBoundRadius(float radius)
    {
        photonView.RPC("RPCChangeRadius", PhotonTargets.All, radius);
    }

    #region RPC

    [PunRPC]
    private void RPCChangeRadius(float radius)
    {
        BoundRadius = radius;
    }

    [PunRPC]
    private void RPCChangeCurrentBrushSize(int width, int height)
    {
        CurrentActive.ChangebrushSize(width, height);
    }

    [PunRPC]
    private void RPCSwitchBrush(int brushNR)
    {
        if (brushNR >= AllBrushes.Count)
            return;

        CurrentActive.gameObject.SetActive(false);
        CurrentActive = AllBrushes[brushNR];
        CurrentActive.gameObject.SetActive(true);
    }

    #endregion RPC
}