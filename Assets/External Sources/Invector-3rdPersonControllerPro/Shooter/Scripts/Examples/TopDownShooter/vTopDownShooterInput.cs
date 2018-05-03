using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController.TopDownShooter
{
    using vShooter;
    [vClassHeader("TopDown Shooter Input")]
    public class vTopDownShooterInput : vShooterMeleeInput
    {
        [vEditorToolbar("Default")]
        public bool alwaysAimForward;
        public float aimMinDistance = 2f;
        private vTopDownController _topDown;

        public vTopDownController topDownController
        {
            get
            {
                if (cc && cc is vTopDownController && _topDown == null) _topDown = cc as vTopDownController;
                return _topDown;
            }
        }

        protected override void UpdateAimPosition()
        {
            if (!topDownController)
            {
                base.UpdateAimPosition();
                return;
            }
            var aimPoint = topDownController.lookPos;
            if (Vector3.Distance(cc._capsuleCollider.bounds.center, aimPoint) < cc.colliderRadius + aimMinDistance)
            {
                aimPoint = transform.position + transform.forward * (cc.colliderRadius + aimMinDistance);
                if (!alwaysAimForward)
                {
                    aimPoint.y = transform.position.y;
                    aimPoint += Vector3.up * Vector3.Distance(transform.position, rightUpperArm.position);
                }
            }
            if (alwaysAimForward)
            {
                aimPoint.y = transform.position.y;
                aimPoint += Vector3.up * Vector3.Distance(transform.position, rightUpperArm.position);
            }
            aimPosition = aimPoint;
        }

        protected override void CheckAimConditions()
        {
            if (!shooterManager || shooterManager.rWeapon == null || !shooterManager.rWeapon.gameObject.activeInHierarchy) return;

            var _ray = new Ray(rightUpperArm.position, aimPosition - (rightUpperArm.position));
            RaycastHit hit;
            if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, out hit, shooterManager.minDistanceToAim, shooterManager.blockAimLayer))
            {
                aimConditions = false;
            }
            else
                aimConditions = true;
            aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 1 * Time.deltaTime);
        }

        protected override Vector3 targetArmAligmentDirection
        {
            get
            {
                return transform.forward;
            }
        }

        protected override Vector3 targetArmAlignmentPosition
        {
            get
            {
                return aimPosition;
            }
        }

        protected override void RotateWithCamera(Transform cameraTransform)
        {
            //  base.RotateWithCamera(cameraTransform);
        }
    }
}



