using UnityEngine;
using System.Collections;
using Invector.vCharacterController;
namespace Invector.vShooter
{
    [vClassHeader("Shooter Lock-On")]
    public class vLockOnShooter : vLockOn
    {
        protected vShooterMeleeInput shooterMelee;

        protected override void Start()
        {
            base.Start();
            shooterMelee = this.tpInput as vShooterMeleeInput;
        }

        protected override void UpdateLockOn(vMeleeCombatInput tpInput)
        {
            if (shooterMelee == null ||
                shooterMelee.shooterManager == null ||
                (shooterMelee.shooterManager.useLockOn && shooterMelee.shooterManager.rWeapon != null) ||
                shooterMelee.shooterManager.useLockOnMeleeOnly && shooterMelee.shooterManager.rWeapon == null)
                base.UpdateLockOn(tpInput);
            else if (shooterMelee.shooterManager.rWeapon != null)
                StopLockOn();
        }

        protected override void LockOnInput()
        {
            if (tpInput.tpCamera == null || tpInput.cc == null) return;
            // lock the camera into a target, if there is any around
            if (lockOnInput.GetButtonDown() && !tpInput.cc.actions)
            {
                isLockingOn = !isLockingOn;
                LockOn(isLockingOn);
            }
            // unlock the camera if the target is null
            else if (isLockingOn && tpInput.tpCamera.lockTarget == null)
            {
                isLockingOn = false;
                LockOn(false);
            }
            // choose to use lock-on with strafe of free movement
            if (strafeWhileLockOn && !tpInput.cc.locomotionType.Equals(vThirdPersonMotor.LocomotionType.OnlyStrafe))
            {
                if (shooterMelee.isAiming || strafeWhileLockOn && isLockingOn && tpInput.tpCamera.lockTarget != null)
                    tpInput.cc.isStrafing = true;
                else
                    tpInput.cc.isStrafing = false;
            }
        }
    }
}