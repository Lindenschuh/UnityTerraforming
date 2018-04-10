using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript;
using UnityStandardAssets.Characters.FirstPerson;

public class NetworkPlayer : Photon.MonoBehaviour {

    public GameObject myCamera;

    // Use this for initialization
    void Start () {
        if(photonView.isMine)
        {
            myCamera.SetActive(true);
            GetComponent<AudioSource>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
        }
        else
        {
            myCamera.SetActive(false);
            GetComponent<AudioSource>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<FirstPersonController>().enabled = false;
        }


	}
	
}
