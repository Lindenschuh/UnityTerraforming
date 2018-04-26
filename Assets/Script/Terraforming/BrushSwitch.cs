﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushSwitch : Photon.PunBehaviour
{
    private List<Brush> AllBrushes = new List<Brush>();
    public List<KeyCode> AllKeys = new List<KeyCode>();
    public Brush CurrentActive;

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
    private void Update()
    {
        for (int i = 0; i < AllKeys.Count; i++)
        {
            if (Input.GetKeyDown(AllKeys[i]))
                photonView.RPC("RPCSwitchBrush", PhotonTargets.All, i);
        }
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
}