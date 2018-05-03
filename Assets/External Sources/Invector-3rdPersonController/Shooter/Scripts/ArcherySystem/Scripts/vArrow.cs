using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vShooter
{
    public class vArrow : MonoBehaviour
    {
        public vProjectileControl projectileControl;
        public Transform detachObject;
        public bool alignToNormal = true;
        [HideInInspector]
        public float penetration;

        public void OnDestroyProjectile(RaycastHit hit)
        {
            detachObject.parent = hit.transform;
            if (alignToNormal)
                detachObject.rotation = Quaternion.LookRotation(-hit.normal);
            detachObject.position = hit.point + transform.forward * penetration;
        }
    }
}