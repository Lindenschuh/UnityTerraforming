using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodUiManager : MonoBehaviour
{
    public GameObject BrushUi;
    public GameObject AbilityUi;
    public GodStateManager GodState;

    // Use this for initialization
    private void Start()
    {
        UpdateUiState();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUiState();
    }

    private void UpdateUiState()
    {
        if (GodState.BrushMng.gameObject.activeSelf)
        {
            AbilityUi.SetActive(false);
            BrushUi.SetActive(true);
        }
        else
        {
            BrushUi.SetActive(false);
            AbilityUi.SetActive(true);
        }
    }
}