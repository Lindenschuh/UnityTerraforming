using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBall : GodAbility
{
    public int DMG;

    public override void UseAbility(Vector3 Impact)
    {
        Collider[] colls = Physics.OverlapSphere(Impact, Radius);
        foreach (Collider col in colls)
        {
            Health h = col.GetComponent<Health>();
            if (h != null)
            {
                h.AddDamage(DMG);
            }
        }
    }
}