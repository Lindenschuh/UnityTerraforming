using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodControlls : MonoBehaviour
{
    public bool Lift { get; private set; }
    public bool Lower { get; private set; }

    // Use this for initialization
    private void Start()
    {
    }

    private void FixedUpdate()
    {
        Lift = Input.GetMouseButton(0);
        Lower = Input.GetMouseButton(1);
    }
}