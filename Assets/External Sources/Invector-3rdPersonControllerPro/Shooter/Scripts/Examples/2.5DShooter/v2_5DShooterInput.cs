using UnityEngine;
using System.Collections;
namespace Invector.vCharacterController.v2_5D
{
    using vShooter;
    [vClassHeader("Shooter 2.5D Input")]
    public class v2_5DShooterInput : vShooterMeleeInput
    {
        [vEditorToolbar("Default")]
        public v2_5DPath path;
        private int forward = 1;
        Vector2 joystickMousePos;
        Vector3 lookDirection;

        private v2_5DController _controller;
        public v2_5DController controller
        {
            get
            {
                if (cc && cc is v2_5DController && _controller == null) _controller = cc as v2_5DController;
                return _controller;
            }
        }

        protected override void Start()
        {
            base.Start();

            path = FindObjectOfType<v2_5DPath>();
            if (path) StartCoroutine(InitPath());
        }

        IEnumerator InitPath()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            transform.position = path.ConstraintPosition(transform.position);
            cc.RotateToDirection(path.reference.right);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!cc.isDead && !cc.ragdolled)
                transform.position = Vector3.Lerp(transform.position, path.ConstraintPosition(transform.position), 80 * Time.deltaTime);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (!isAiming && !cc.isStrafing && !cc.customAction && path && cc.input.magnitude > 0.1f) cc.RotateToDirection(path.reference.right * cc.input.x);
        }

        protected override bool IsAimAlignWithForward()
        {
            return true;
        }

        protected override void UpdateAimPosition()
        {
            if (!isAiming || !controller) return;
            var lookPos = controller.lookPos;

            var localPos = transform.InverseTransformPoint(lookPos);
            localPos.x = 0;
            if (localPos.z < -0.2f)
            {
                if (localPos.z > -0.4f)
                    localPos.z = -0.4f;
                forward *= -1;
            }
            else if (localPos.z > 0.2f && localPos.z < 0.4f)
            {
                lookPos.z = 0.3f;
            }
            transform.forward = path.reference.right * forward;
            var wordPos = transform.TransformPoint(localPos);

            lookDirection = wordPos - rightUpperArm.position;
            aimPosition = rightUpperArm.position + lookDirection;
            headTrack.SetTemporaryLookPoint(aimPosition);
        }

        protected override Vector3 targetArmAligmentDirection
        {
            get
            {
                return transform.forward;
            }
        }

    }
}