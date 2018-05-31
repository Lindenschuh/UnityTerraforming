using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodData : MonoBehaviour
{
    public float MaxGauge;
    public float ManipulationValue;
    public float RegenerationRate;
    public float LiftGauge;

    private float epsilon = 0.001f;

    private void Start()
    {
        LiftGauge = MaxGauge;
    }

    private void FixedUpdate()
    {
        LiftGauge = Mathf.Clamp(LiftGauge + RegenerationRate * Time.fixedDeltaTime, 0, MaxGauge);
    }

    public bool Lift()
    {
        LiftGauge = Mathf.Clamp(LiftGauge - ManipulationValue * Time.fixedDeltaTime, 0, MaxGauge);
        return LiftGauge > 0 + epsilon;
    }

    public bool Lower()
    {
        LiftGauge = Mathf.Clamp(LiftGauge - ManipulationValue * Time.fixedDeltaTime, 0, MaxGauge);
        return LiftGauge > 0 + epsilon;
    }
}