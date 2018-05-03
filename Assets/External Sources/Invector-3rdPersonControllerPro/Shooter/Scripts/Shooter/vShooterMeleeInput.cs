using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.vShooter
{
    using vCharacterController;
    using IK;
    [vClassHeader("SHOOTER/MELEE INPUT", iconName = "inputIcon")]
    public class vShooterMeleeInput : vMeleeCombatInput
    {
        #region Shooter Inputs

        [vEditorToolbar("Inputs")]
        [Header("Shooter Inputs")]
        public GenericInput aimInput = new GenericInput("Mouse1", false, "LT", true, "LT", false);
        public GenericInput shotInput = new GenericInput("Mouse0", false, "RT", true, "RT", false);
        public GenericInput secundaryShotInput = new GenericInput("Mouse2", false, "X", true, "X", false);
        public GenericInput reloadInput = new GenericInput("R", "LB", "LB");
        public GenericInput switchCameraSideInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
        public GenericInput switchScopeViewInput = new GenericInput("Z", "RB", "RB");

        #endregion

        #region Shooter Variables       

        [HideInInspector]
        public vShooterManager shooterManager;
        [HideInInspector]
        public bool blockAim;
        [HideInInspector]
        public bool isAiming;
        [HideInInspector]
        public bool canEquip;
        [HideInInspector]
        public bool isReloading;
        [HideInInspector]
        public bool isEquipping;
        [HideInInspector]
        public Transform leftHand, rightHand, rightUpperArm, leftUpperArm;
        [HideInInspector]
        public Vector3 aimPosition;

        protected int onlyArmsLayer;
        protected bool allowAttack;
        protected bool aimConditions;
        protected bool isUsingScopeView;
        protected bool isCameraRightSwitched;
        protected float onlyArmsLayerWeight;
        protected float lIKWeight;
        protected float rightRotationWeight;
        protected float aimWeight;
        protected float aimTimming;
        protected float lastAimDistance;
        protected Quaternion handRotation, upperArmRotation;
        protected vIKSolver leftIK, rightIK;
        protected vHeadTrack headTrack;
        private vControlAimCanvas _controlAimCanvas;
        private GameObject aimAngleReference;
        private Vector3 ikRotationOffset;
        private Vector3 ikPositionOffset;
        public vControlAimCanvas controlAimCanvas
        {
            get
            {
                if (!_controlAimCanvas)
                    _controlAimCanvas = FindObjectOfType<vControlAimCanvas>();
                return _controlAimCanvas;
            }
        }

        [HideInInspector]
        public bool lockShooterInput;

        #endregion

        protected override void Start()
        {
            shooterManager = GetComponent<vShooterManager>();

            base.Start();

            leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            onlyArmsLayer = animator.GetLayerIndex("OnlyArms");
            aimAngleReference = new GameObject("aimAngleReference");
            aimAngleReference.transform.rotation = transform.rotation;
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            aimAngleReference.transform.SetParent(chest);
            aimAngleReference.transform.localPosition = Vector3.zero;

            headTrack = GetComponent<vHeadTrack>();

            if (!controlAimCanvas)
                Debug.LogWarning("Missing the AimCanvas, drag and drop the prefab to this scene in order to Aim", gameObject);
        }

        protected override void LateUpdate()
        {
            if ((!updateIK && animator.updateMode == AnimatorUpdateMode.AnimatePhysics)) return;
            base.LateUpdate();
            UpdateAimBehaviour();                       
        }       

        #region Shooter Inputs    

        public void SetLockShooterInput(bool value)
        {
            lockShooterInput = value;

            if (value)
            {
                cc.isStrafing = false;
                isBlocking = false;
                isAiming = false;
                aimTimming = 0f;
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
            }
        }

        protected override void InputHandle()
        {
            if (cc == null || lockInput || cc.isDead)
                return;

            #region MeleeInput

            if (MeleeAttackConditions && !isAiming && !isReloading && !lockMeleeInput)
            {
                MeleeWeakAttackInput();
                MeleeStrongAttackInput();
                BlockingInput();
            }
            else
                isBlocking = false;

            #endregion

            #region BasicInput

            if (!isAttacking)
            {
                if (!cc.lockMovement && !cc.ragdolled)
                {
                    MoveCharacter();
                    SprintInput();
                    CrouchInput();
                    StrafeInput();
                    JumpInput();
                    RollInput();
                }

                UpdateMeleeAnimations();
            }
            else
                cc.input = Vector2.zero;

            #endregion

            #region ShooterInput

            if (lockShooterInput)
            {
                isAiming = false;
            }
            else
            {
                if (shooterManager == null || ((shooterManager.rWeapon == null || !shooterManager.rWeapon.gameObject.activeInHierarchy) && shooterManager.lWeapon == null))
                {
                    isAiming = false;
                    if (controlAimCanvas != null)
                    {
                        controlAimCanvas.SetActiveAim(false);
                        controlAimCanvas.SetActiveScopeCamera(false);
                    }
                }
                else
                {
                    AimInput();
                    ReloadInput();
                    SwitchCameraSide();
                    SwitchScopeViewInput();
                }
            }
            onUpdateInput.Invoke(this);
            #endregion
        }

        public override bool lockInventory
        {
            get
            {
                return base.lockInventory || isReloading;
            }
        }

        protected virtual void AimInput()
        {
            if (!shooterManager)
            {
                isAiming = false;
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
                if (cc.isStrafing) cc.Strafe();
                return;
            }
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;

            if (cc.locomotionType == vThirdPersonMotor.LocomotionType.OnlyFree)
            {
                Debug.LogWarning("Shooter behaviour needs to be OnlyStrafe or Free with Strafe. \n Please change the Locomotion Type.");
                return;
            }

            if (shooterManager.hipfireShot)
            {
                if (aimTimming > 0)
                    aimTimming -= Time.deltaTime;
            }

            if (!shooterManager || !weapon)
            {
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
                isAiming = false;
                if (cc.isStrafing) cc.Strafe();
                return;
            }

            if (!cc.isRolling)
                isAiming = !isReloading && (aimInput.GetButton() || (shooterManager.alwaysAiming)) && !cc.ragdolled && !cc.actions && !cc.customAction || (cc.actions && cc.isJumping);

            if (headTrack)
                headTrack.awaysFollowCamera = isAiming;

            if (cc.locomotionType == vThirdPersonMotor.LocomotionType.FreeWithStrafe)
            {
                if ((isAiming || aimTimming > 0) && !cc.isStrafing)
                {
                    cc.Strafe();
                }
                else if ((!isAiming && aimTimming <= 0) && cc.isStrafing)
                {
                    cc.Strafe();
                }
            }

            if (controlAimCanvas)
            {
                if ((isAiming || aimTimming > 0) && !controlAimCanvas.isAimActive)
                    controlAimCanvas.SetActiveAim(true);
                if ((!isAiming && aimTimming <= 0) && controlAimCanvas.isAimActive)
                    controlAimCanvas.SetActiveAim(false);
            }
            if (shooterManager.rWeapon)
            {
                shooterManager.rWeapon.SetActiveAim(isAiming && aimConditions);
                shooterManager.rWeapon.SetActiveScope(isAiming && isUsingScopeView);
            }
            else if (shooterManager.lWeapon)
            {
                shooterManager.lWeapon.SetActiveAim(isAiming && aimConditions);
                shooterManager.lWeapon.SetActiveScope(isAiming && isUsingScopeView);
            }
        }

        protected virtual void ShotInput()
        {
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (!shooterManager || weapon == null || cc.isDead) return;

            if ((isAiming && !shooterManager.hipfireShot || shooterManager.hipfireShot) && !shooterManager.isShooting && aimConditions && !isReloading && !isAttacking)
            {
                HandleShot(weapon, shotInput);
                if (weapon.secundaryWeapon)
                {
                    HandleShot(weapon.secundaryWeapon, secundaryShotInput, true);
                }
            }
            else if (!isAiming)
            {
                if (weapon.chargeWeapon && weapon.powerCharge != 0) weapon.powerCharge = 0;
                if (weapon.secundaryWeapon != null && weapon.secundaryWeapon.chargeWeapon && weapon.secundaryWeapon.powerCharge != 0) weapon.secundaryWeapon.powerCharge = 0;
            }
            shooterManager.UpdateShotTime();
        }

        protected virtual void HandleShot(vShooterWeapon weapon, GenericInput weaponInput, bool secundaryShot = false)
        {
            if (weapon.chargeWeapon)
            {
                if (weapon.ammoCount > 0 && weapon.powerCharge < 1 && weaponInput.GetButton()) weapon.powerCharge += Time.deltaTime * weapon.chargeSpeed;
                else if ((weapon.powerCharge >= 1 && weapon.autoShotOnFinishCharge) || weaponInput.GetButtonUp() || (!weaponInput.GetButton() && isAiming && weapon.powerCharge > 0))
                {
                    if (shooterManager.hipfireShot) aimTimming = 3f;
                    shooterManager.Shoot(aimPosition, !isAiming, secundaryShot);
                    weapon.powerCharge = 0;
                }
                animator.SetFloat("PowerCharger", weapon.powerCharge);
            }
            else if (weapon.automaticWeapon ? weaponInput.GetButton() : weaponInput.GetButtonDown())
            {
                if (shooterManager.hipfireShot) aimTimming = 3f;
                shooterManager.Shoot(aimPosition, !isAiming, secundaryShot);
            }
            else if (weaponInput.GetButtonDown())
            {
                if (allowAttack == false)
                {
                    if (shooterManager.hipfireShot) aimTimming = 1f;
                    shooterManager.Shoot(aimPosition, !isAiming, secundaryShot);
                    allowAttack = true;
                }
            }
            else allowAttack = false;
        }

        protected virtual void ReloadInput()
        {
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (!shooterManager || weapon == null) return;
            if (reloadInput.GetButtonDown() && !cc.actions && !cc.ragdolled)
                shooterManager.ReloadWeapon();
        }

        protected virtual void SwitchCameraSide()
        {
            if (tpCamera == null) return;
            if (switchCameraSideInput.GetButtonDown())
            {
                isCameraRightSwitched = !isCameraRightSwitched;
                tpCamera.SwitchRight(isCameraRightSwitched);
            }
        }

        protected virtual void SwitchScopeViewInput()
        {
            if (!shooterManager || shooterManager.rWeapon == null) return;
            if (isAiming && aimConditions && switchScopeViewInput.GetButtonDown())
            {
                if (controlAimCanvas && shooterManager.rWeapon.scopeTarget)
                {
                    isUsingScopeView = !isUsingScopeView;
                    controlAimCanvas.SetActiveScopeCamera(isUsingScopeView, shooterManager.rWeapon.useUI);
                }
            }
            else if (controlAimCanvas && !isAiming || controlAimCanvas && !aimConditions)
            {
                isUsingScopeView = false;
                controlAimCanvas.SetActiveScopeCamera(false);
            }
        }

        protected override void BlockingInput()
        {
            if (shooterManager == null || shooterManager.rWeapon == null)
                base.BlockingInput();
        }

        protected override void RotateWithCamera(Transform cameraTransform)
        {
            if (cc.isStrafing && !cc.actions && !cc.lockMovement && rotateToCameraWhileStrafe)
            {
                // smooth align character with aim position
                if (tpCamera != null && tpCamera.lockTarget)
                {
                    cc.RotateToTarget(tpCamera.lockTarget);
                }
                // rotate the camera around the character and align with when the char move
                else if (cc.input != Vector2.zero || (isAiming || aimTimming > 0))
                {
                    cc.RotateWithAnotherTransform(cameraTransform);
                }
            }
        }

        #endregion

        #region Update Animations

        protected override void UpdateMeleeAnimations()
        {
            // disable the onlyarms layer and run the melee methods if the character is not using any shooter weapon
            if (!animator) return;

            // update MeleeManager Animator Properties
            if ((shooterManager == null || (shooterManager.rWeapon == null && shooterManager.lWeapon == null)) && meleeManager)
            {
                base.UpdateMeleeAnimations();
                // set the uppbody id (armsonly layer)
                animator.SetFloat("UpperBody_ID", 0, .2f, Time.deltaTime);
                // turn on the onlyarms layer to aim 
                onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0, 6f * Time.deltaTime);
                animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
                // reset aiming parameter
                animator.SetBool("IsAiming", false);
                isReloading = false;
            }
            // update ShooterManager Animator Properties
            else if (shooterManager && (shooterManager.rWeapon || shooterManager.lWeapon))
                UpdateShooterAnimations();
            // reset Animator Properties
            else
            {
                // set the move set id (base layer) 
                animator.SetFloat("MoveSet_ID", 0, .2f, Time.deltaTime);
                // set the uppbody id (armsonly layer)
                animator.SetFloat("UpperBody_ID", 0, .2f, Time.deltaTime);
                // set if the character can aim or not (upperbody layer)
                animator.SetBool("CanAim", false);
                // character is aiming
                animator.SetBool("IsAiming", false);
                // turn on the onlyarms layer to aim 
                onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0, 6f * Time.deltaTime);
                animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
            }
        }

        protected virtual void UpdateShooterAnimations()
        {
            if (shooterManager == null) return;

            if ((!isAiming && aimTimming <= 0) && meleeManager)
            {
                // set attack id from the melee weapon (trigger fullbody atk animations)
                animator.SetInteger("AttackID", meleeManager.GetAttackID());
            }
            else
            {
                // set attack id from the shooter weapon (trigger shot layer animations)
                animator.SetInteger("AttackID", shooterManager.GetAttackID());
            }
            // turn on the onlyarms layer to aim 
            onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, (isAiming || aimTimming > 0) ? 0f : (shooterManager.rWeapon || shooterManager.lWeapon) ? 1f : 0f, 6f * Time.deltaTime);
            animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (weapon != null && (isAiming || aimTimming > 0))
            {
                // set the move set id (base layer) 
                animator.SetFloat("MoveSet_ID", shooterManager.GetMoveSetID(), .2f, Time.deltaTime);
            }
            else if (weapon != null)
            {
                // set the move set id (base layer) 
                animator.SetFloat("MoveSet_ID", 0, .2f, Time.deltaTime);
            }
            // set the isBlocking false while using shooter weapons
            animator.SetBool("IsBlocking", false);
            // set the uppbody id (armsonly layer)
            animator.SetFloat("UpperBody_ID", shooterManager.GetUpperBodyID(), .2f, Time.deltaTime);
            // set if the character can aim or not (upperbody layer)
            animator.SetBool("CanAim", aimConditions);
            // character is aiming
            animator.SetBool("IsAiming", (isAiming || aimTimming > 0) && !isAttacking);
            // find states with the Reload tag
            isReloading = cc.IsAnimatorTag("Reload");
            // find states with the IsEquipping tag
            isEquipping = cc.IsAnimatorTag("IsEquipping");
        }

        protected override void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vCamera.vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }

            if (changeCameraState)
                tpCamera.ChangeState(customCameraState, customlookAtPoint, true);
            else if (cc.isCrouching)
                tpCamera.ChangeState("Crouch", true);
            else if (cc.isStrafing && !isAiming)
                tpCamera.ChangeState("Strafing", true);
            else if (isAiming && (shooterManager.rWeapon || shooterManager.lWeapon))
                tpCamera.ChangeState("Aiming", true);
            else
                tpCamera.ChangeState("Default", true);
        }

        #endregion

        #region Update Aim

        protected virtual void UpdateAimPosition()
        {
            if (!shooterManager) return;
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (weapon == null) return;

            var camT = isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeCamera ? //Check if is using canvas scope view
                    weapon.zoomScopeCamera ? /* if true, check if weapon has a zoomScopeCamera, 
                if true...*/
                    weapon.zoomScopeCamera.transform : controlAimCanvas.scopeCamera.transform :
                    /*else*/Camera.main.transform;

            var origin1 = camT.position;
            if (!(controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera))
                origin1 = camT.position;

            var vOrigin = origin1;
            vOrigin += controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? camT.forward : Vector3.zero;
            aimPosition = camT.position + camT.forward * 100f;
            //aimAngleReference.transform.eulerAngles = new Vector3(aimAngleReference.transform.eulerAngles.x, transform.eulerAngles.y, aimAngleReference.transform.eulerAngles.z);
            if (!isUsingScopeView) lastAimDistance = 100f;

            if (shooterManager.raycastAimTarget && weapon.raycastAimTarget)
            {
                RaycastHit hit;
                Ray ray = new Ray(vOrigin, camT.forward);

                if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, shooterManager.damageLayer))
                {
                    if (hit.collider.transform.IsChildOf(transform))
                    {
                        var collider = hit.collider;
                        var hits = Physics.RaycastAll(ray, Camera.main.farClipPlane, shooterManager.damageLayer);
                        var dist = Camera.main.farClipPlane;
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].distance < dist && hits[i].collider.gameObject != collider.gameObject && !hits[i].collider.transform.IsChildOf(transform))
                            {
                                dist = hits[i].distance;
                                hit = hits[i];
                            }
                        }
                    }

                    if (hit.collider)
                    {
                        if (!isUsingScopeView)
                            lastAimDistance = Vector3.Distance(camT.position, hit.point);
                        aimPosition = hit.point;
                    }
                }
                if (shooterManager.showCheckAimGizmos)
                {
                    Debug.DrawLine(ray.origin, aimPosition);
                }
            }
            if (isAiming)
                shooterManager.CameraSway();
        }

        #endregion

        #region IK behaviour

        void OnDrawGizmos()
        {
            if (!shooterManager || !shooterManager.showCheckAimGizmos) return;
            var weaponSide = isCameraRightSwitched ? -1 : 1;
            var _ray = new Ray(aimAngleReference.transform.position + transform.up * shooterManager.blockAimOffsetY + transform.right * shooterManager.blockAimOffsetX * weaponSide, Camera.main.transform.forward);
            Gizmos.DrawRay(_ray.origin, _ray.direction * shooterManager.minDistanceToAim);
            var color = Gizmos.color;
            color = aimConditions ? Color.green : Color.red;
            color.a = 1f;
            Gizmos.color = color;
            Gizmos.DrawSphere(_ray.GetPoint(shooterManager.minDistanceToAim), shooterManager.checkAimRadius);
            Gizmos.DrawSphere(aimPosition, shooterManager.checkAimRadius);
            
        }

        protected virtual void UpdateAimBehaviour()
        {
            UpdateAimPosition();
            UpdateHeadTrack();
            if (shooterManager && shooterManager.rWeapon)
            {               
                RotateAimArm();
                RotateAimHand();

                if (!shooterManager.lWeapon)
                    UpdateLeftIK();
            }
            if (shooterManager && shooterManager.lWeapon)
            {
                RotateAimArm(true);
                RotateAimHand(true);
                if (!shooterManager.rWeapon)
                    UpdateLeftIK(true);
            }
            if (isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeCamera) UpdateAimPosition();
            CheckAimConditions();
            UpdateAimHud();
            ShotInput();
        }

        protected virtual void UpdateLeftIK(bool isUsingLeftHand = false)
        {
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (!shooterManager || !weapon || !weapon.gameObject.activeInHierarchy || !shooterManager.useLeftIK) return;
            if (animator.GetCurrentAnimatorStateInfo(6).IsName("Shot Fire") && weapon.disableIkOnShot) { lIKWeight = 0; return; }

            bool useIkConditions = false;
            var animatorInput = cc.isStrafing ? new Vector2(animator.GetFloat("InputVertical"), animator.GetFloat("InputHorizontal")).magnitude : animator.GetFloat("InputVertical");
            if (!isAiming && !isAttacking)
            {
                if (animatorInput < 0.1f)
                    useIkConditions = weapon.useIkOnIdle;
                else if (cc.isStrafing)
                    useIkConditions = weapon.useIkOnStrafe;
                else useIkConditions = weapon.useIkOnFree;
            }
            else if (isAiming && !isAttacking) useIkConditions = weapon.useIKOnAiming;
            else if (isAttacking) useIkConditions = weapon.useIkAttacking;

            // create left arm ik solver if equal null
            if (leftIK == null) leftIK = new vIKSolver(animator, AvatarIKGoal.LeftHand);
            if (rightIK == null) rightIK = new vIKSolver(animator, AvatarIKGoal.RightHand);
            vIKSolver targetIK = null;
            if (isUsingLeftHand)
            {
                targetIK = rightIK;
            }
            else targetIK = leftIK;

            if (targetIK != null)
            {
                if(isUsingLeftHand)
                {
                    ikRotationOffset = shooterManager.ikRotationOffsetR;
                    ikPositionOffset = shooterManager.ikPositionOffsetR;
                }
                else
                {
                    ikRotationOffset = shooterManager.ikRotationOffsetL;
                    ikPositionOffset = shooterManager.ikPositionOffsetL;
                }
                // control weight of ik
                if (weapon && weapon.handIKTarget && Time.timeScale > 0 && !isReloading && !cc.actions && !cc.customAction && !isEquipping && (cc.isGrounded || (isAiming || aimTimming > 0f)) && !cc.lockMovement && useIkConditions)
                    lIKWeight = Mathf.Lerp(lIKWeight, 1, 10f * Time.deltaTime);
                else
                    lIKWeight = Mathf.Lerp(lIKWeight, 0, 10f * Time.deltaTime);

                if (lIKWeight <= 0) return;
                // update IK
                targetIK.SetIKWeight(lIKWeight);
                if (shooterManager && weapon && weapon.handIKTarget)
                {
                    var _offset = (weapon.handIKTarget.forward * ikPositionOffset.z) + (weapon.handIKTarget.right * ikPositionOffset.x) + (weapon.handIKTarget.up * ikPositionOffset.y);
                    targetIK.SetIKPosition(weapon.handIKTarget.position + _offset);
                    var _rotation = Quaternion.Euler(ikRotationOffset);
                    targetIK.SetIKRotation(weapon.handIKTarget.rotation * _rotation);
                }
            }
        }
     
        protected virtual void RotateAimArm(bool isUsingLeftHand = false)
        {
            if (!shooterManager) return;

            var weapon = !isUsingLeftHand ? shooterManager.rWeapon : shooterManager.lWeapon;

            if (weapon && weapon.gameObject.activeInHierarchy && (isAiming || aimTimming > 0f) && aimConditions && weapon.alignRightUpperArmToAim)
            {
                var aimPoint = targetArmAlignmentPosition;
                Vector3 v = aimPoint - weapon.aimReference.position;
                Vector3 v2 = Quaternion.AngleAxis(-weapon.recoilUp, weapon.aimReference.right) * v;
                var orientation = weapon.aimReference.forward;
                rightRotationWeight = Mathf.Lerp(rightRotationWeight, !shooterManager.isShooting || weapon.ammoCount <= 0 ? 1f * aimWeight : 0f, 1f * Time.deltaTime);
                var upperArm = isUsingLeftHand ? leftUpperArm : rightUpperArm;
                var r = Quaternion.FromToRotation(orientation, v) * upperArm.rotation;
                var r2 = Quaternion.FromToRotation(orientation, v2) * upperArm.rotation;
                Quaternion rot = Quaternion.Lerp(r2, r, rightRotationWeight);
                var angle = Vector3.Angle(aimPosition - aimAngleReference.transform.position, aimAngleReference.transform.forward);

                if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || controlAimCanvas && controlAimCanvas.isScopeCameraActive)
                    upperArmRotation = Quaternion.Lerp(upperArmRotation, rot, shooterManager.smoothArmIKRotation * Time.deltaTime);
                else upperArmRotation = upperArm.rotation;

                if (!float.IsNaN(upperArmRotation.x) && !float.IsNaN(upperArmRotation.y) && !float.IsNaN(upperArmRotation.z))
                    upperArm.rotation = upperArmRotation;
            }
        }

        protected virtual void RotateAimHand(bool isUsingLeftHand = false)
        {
            if (!shooterManager) return;
            var weapon = !isUsingLeftHand ? shooterManager.rWeapon : shooterManager.lWeapon;

            if (weapon && weapon.gameObject.activeInHierarchy && weapon.alignRightHandToAim && (isAiming || aimTimming > 0f) && aimConditions)
            {
                var aimPoint = targetArmAlignmentPosition;
                Vector3 v = aimPoint - weapon.aimReference.position;
                Vector3 v2 = Quaternion.AngleAxis(-weapon.recoilUp, weapon.aimReference.right) * v;
                var orientation = weapon.aimReference.forward;

                if (!weapon.alignRightUpperArmToAim)
                    rightRotationWeight = Mathf.Lerp(rightRotationWeight, !shooterManager.isShooting || weapon.ammoCount <= 0 ? 1f * aimWeight : 0f, 1f * Time.deltaTime);

                var hand = isUsingLeftHand ? leftHand : rightHand;
                var r = Quaternion.FromToRotation(orientation, v) * hand.rotation;
                var r2 = Quaternion.FromToRotation(orientation, v2) * hand.rotation;
                Quaternion rot = Quaternion.Lerp(r2, r, rightRotationWeight);
                var angle = Vector3.Angle(aimPosition - aimAngleReference.transform.position, aimAngleReference.transform.forward);

                if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || (controlAimCanvas && controlAimCanvas.isScopeCameraActive))
                    handRotation = Quaternion.Lerp(handRotation, rot, shooterManager.smoothArmIKRotation * Time.deltaTime);
                else handRotation = Quaternion.Lerp(hand.rotation, rot, shooterManager.smoothArmIKRotation * Time.deltaTime);

                if (!float.IsNaN(handRotation.x) && !float.IsNaN(handRotation.y) && !float.IsNaN(handRotation.z))
                    hand.rotation = handRotation;

                weapon.SetScopeLookTarget(aimPoint);
            }
        }

        protected virtual void CheckAimConditions()
        {
            if (!shooterManager) return;
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            var weaponSide = isCameraRightSwitched ? -1 : 1;

            if (weapon == null || !weapon.gameObject.activeInHierarchy)
            {
                aimConditions = false;
                return;
            }
            if (!shooterManager.hipfireShot && !IsAimAlignWithForward())
            {
                aimConditions = false;
            }
            else
            {
                var _ray = new Ray(aimAngleReference.transform.position + transform.up * shooterManager.blockAimOffsetY + transform.right * shooterManager.blockAimOffsetX * weaponSide, Camera.main.transform.forward);
                RaycastHit hit;
                if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, out hit, shooterManager.minDistanceToAim, shooterManager.blockAimLayer))
                {
                    aimConditions = false;
                }
                else
                    aimConditions = true;
            }

            aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 10 * Time.deltaTime);
        }

        protected virtual bool IsAimAlignWithForward()
        {
            if (!shooterManager) return false;
            var angle = Quaternion.LookRotation(aimPosition - aimAngleReference.transform.position, Vector3.up).eulerAngles - transform.eulerAngles;

            return ((angle.NormalizeAngle().y < 90 && angle.NormalizeAngle().y > -90));
        }

        protected virtual Vector3 targetArmAlignmentPosition
        {
            get
            {
                return isUsingScopeView && controlAimCanvas.scopeCamera ? Camera.main.transform.position + Camera.main.transform.forward * lastAimDistance : aimPosition;
            }
        }

        protected virtual Vector3 targetArmAligmentDirection
        {
            get
            {
                var t = controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? controlAimCanvas.scopeCamera.transform : Camera.main.transform;
                return t.forward;
            }
        }

        protected virtual void UpdateHeadTrack()
        {
            if (!shooterManager || !headTrack)
            {
                if (headTrack) headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.smooth * Time.deltaTime);
                return;
            }
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (!weapon || !weapon.gameObject.activeInHierarchy || !headTrack)
            {
                if (headTrack) headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.smooth * Time.deltaTime);
                return;
            }
            if (isAiming || aimTimming > 0f)
            {
                var offset = cc.isCrouching ? weapon.headTrackOffsetCrouch : weapon.headTrackOffset;
                headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, offset, headTrack.smooth * Time.deltaTime);
            }
            else
            {
                headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.smooth * Time.deltaTime);
            }
        }

        protected virtual void UpdateAimHud()
        {
            if (!shooterManager || !controlAimCanvas) return;
            var weapon = shooterManager.rWeapon ? shooterManager.rWeapon : shooterManager.lWeapon;
            if (weapon == null || !weapon.gameObject.activeInHierarchy) return;
            controlAimCanvas.SetAimCanvasID(weapon.scopeID);
            if (controlAimCanvas.scopeCamera && controlAimCanvas.scopeCamera.gameObject.activeSelf)
                controlAimCanvas.SetAimToCenter(true);
            else if (isAiming)
            {
                RaycastHit hit;
                if (Physics.Linecast(weapon.muzzle.position, aimPosition, out hit, shooterManager.blockAimLayer))
                    controlAimCanvas.SetWordPosition(hit.point, aimConditions);
                else
                    controlAimCanvas.SetWordPosition(aimPosition, aimConditions);

            }
            else
                controlAimCanvas.SetAimToCenter(true);

            if (weapon.scopeTarget)
            {
                var lookPoint = Camera.main.transform.position + (Camera.main.transform.forward * (isUsingScopeView ? lastAimDistance : 100f));
                controlAimCanvas.UpdateScopeCamera(weapon.scopeTarget.position, lookPoint, weapon.zoomScopeCamera ? 0 : weapon.scopeZoom);
            }
        }

        #endregion
    }
}