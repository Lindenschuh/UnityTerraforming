using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGodGaugeManager : MonoBehaviour
{
    public GodData GD;
    public RectTransform BackGround;
    public RectTransform Gauge;

    private float MaxWidth;

    // Use this for initialization
    private void Start()
    {
        MaxWidth = BackGround.rect.width;
    }

    // Update is called once per frame
    private void Update()
    {
        float currentGaugePercent = GD.LiftGauge / GD.MaxGauge;
        Gauge.sizeDelta = new Vector2(MaxWidth * currentGaugePercent, Gauge.sizeDelta.y);
    }
}