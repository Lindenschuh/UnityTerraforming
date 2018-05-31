using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUiManager : MonoBehaviour
{
    public AbilityManager AS;
    public Color NotLit;
    public Color Lit;

    private Image[] uiBackGrounds;
    private int currentActiveAbilityIndex;

    // Use this for initialization
    private void Start()
    {
        uiBackGrounds = GetComponentsInChildren<Image>();
        currentActiveAbilityIndex = AS.CurrentActiveIndex;
        uiBackGrounds.ToList().ForEach(i => i.color = NotLit);
        UpdateHighLights(currentActiveAbilityIndex);
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentActiveAbilityIndex != AS.CurrentActiveIndex)
            UpdateHighLights(AS.CurrentActiveIndex);
    }

    private void UpdateHighLights(int currentActiveIndex)
    {
        if (currentActiveIndex >= uiBackGrounds.Length)
            return;

        uiBackGrounds[currentActiveAbilityIndex].color = NotLit;
        uiBackGrounds[currentActiveIndex].color = Lit;
        currentActiveAbilityIndex = currentActiveIndex;
    }
}