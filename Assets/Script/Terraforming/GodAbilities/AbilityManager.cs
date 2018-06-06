using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : Photon.PunBehaviour
{
    public List<KeyCode> Codes = new List<KeyCode>();
    private List<GodAbility> Abilities = new List<GodAbility>();
    public GodAbility CurrentAbility { get; private set; }
    public int CurrentActiveIndex { get; private set; }

    private void Start()
    {
        Abilities.AddRange(GetComponentsInChildren<GodAbility>());
        if (Abilities.Count > 0)
        {
            Abilities.ForEach(b => b.gameObject.SetActive(false));
            CurrentAbility = Abilities[0];
            CurrentAbility.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < Codes.Count; i++)
        {
            if (Input.GetKeyDown(Codes[i]))
            {
                photonView.RPC("RPCSwitchAbility", PhotonTargets.All, i);
            }
        }
    }

    [PunRPC]
    private void RPCSwitchAbility(int number)
    {
        if (number >= Abilities.Count)
            return;

        CurrentAbility.gameObject.SetActive(false);
        CurrentAbility = Abilities[number];
        CurrentActiveIndex = number;
        CurrentAbility.gameObject.SetActive(true);
    }
}