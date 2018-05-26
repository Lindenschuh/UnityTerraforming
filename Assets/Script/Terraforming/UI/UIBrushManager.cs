using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBrushManager : MonoBehaviour
{
    public BrushSwitch BS;
    public Color NotLit;
    public Color Lit;

    private Image[] uiBackGrounds;
    private int currentActiveBrushIndex;

    // Use this for initialization
    private void Start()
    {
        uiBackGrounds = GetComponentsInChildren<Image>();
        currentActiveBrushIndex = BS.CurrentActiveIndex;
        uiBackGrounds.ToList().ForEach(i => i.color = NotLit);
        UpdateHighLights(currentActiveBrushIndex);
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentActiveBrushIndex != BS.CurrentActiveIndex)
            UpdateHighLights(BS.CurrentActiveIndex);
    }

    private void UpdateHighLights(int currentActiveIndex)
    {
        if (currentActiveIndex >= uiBackGrounds.Length)
            return;

        uiBackGrounds[currentActiveBrushIndex].color = NotLit;
        uiBackGrounds[currentActiveIndex].color = Lit;
        currentActiveBrushIndex = currentActiveIndex;
    }
}