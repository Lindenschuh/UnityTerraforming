﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBound : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other);
    }
}