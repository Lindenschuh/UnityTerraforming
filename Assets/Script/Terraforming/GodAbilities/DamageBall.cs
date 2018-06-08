using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBall : GodAbility
{
    public float DMGRadius;

    public override void UseAbility(Vector3 Impact)
    {
        Collider[] colls = Physics.OverlapSphere(Impact, DMGRadius);
        foreach (Collider col in colls)
        {
        }
    }
}