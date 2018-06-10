using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GodAbility : MonoBehaviour
{
    public float Width, Height;

    public abstract void UseAbility(Vector3 Impact);
}