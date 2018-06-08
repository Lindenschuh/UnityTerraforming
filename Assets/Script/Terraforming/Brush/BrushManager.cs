﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushManager : Photon.PunBehaviour
{
    private List<Brush> AllBrushes = new List<Brush>();
    public List<KeyCode> AllKeys = new List<KeyCode>();

    public int CurrentActiveIndex { get; private set; }
    public Brush CurrentActive { get; private set; }

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

    #region RPC

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
        CurrentActiveIndex = brushNR;
        CurrentActive.gameObject.SetActive(true);
    }

    #endregion RPC
}