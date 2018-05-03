using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Invector.vShooter
{
    using vItemManager;
    using vCharacterController;
    [vClassHeader("SHOOTER MANAGER", iconName = "shooterIcon")]
    public class vShooterManager : vMonoBehaviour
    {
        #region variables

        [System.Serializable]
        public class OnReloadWeapon : UnityEngine.Events.UnityEvent<vShooterWeapon> { }
        public delegate void TotalAmmoHandler(int ammoID, ref int ammo);

        [vEditorToolbar("Damage Layers")]
        [Tooltip("Layer to aim and apply damage")]
        public LayerMask damageLayer = 1 << 0;
        [Tooltip("Tags to ignore (auto add this gameObject tag to avoid damage your self)")]
        public vTagMask ignoreTags = new vTagMask("Player");
        [Tooltip("Layer to block aim")]
        public LayerMask blockAimLayer = 1 << 0;
        public float blockAimOffsetY = 0.35f;
        public float blockAimOffsetX = 0.35f;

        [vEditorToolbar("Aim")]
        [Header("- Float Values")]
        [Tooltip("min distance to aim")]
        public float minDistanceToAim = 1;
        public float checkAimRadius = 0.1f;
        [Tooltip("Check true to make the character always aim and walk on strafe mode")]
        [Header("- Shooter Settings")]
        public bool alwaysAiming;

        [vEditorToolbar("IK")]
        [Tooltip("smooth of the right hand when correcting the aim")]
        public float smoothArmIKRotation = 30f;
        [Tooltip("Limit the maxAngle for the right hand to correct the aim")]
        public float maxAimAngle = 60f;
        [Tooltip("Check this to syinc the weapon aim to the camera aim")]
        public bool raycastAimTarget = true;
        [Tooltip("Move camera angle when shot using recoil properties of weapon")]
        public bool applyRecoilToCamera = true;
        [Tooltip("Check this to use IK on the left hand")]
        public bool useLeftIK = true, useRightIK = true;
        [Tooltip("Instead of adjust each weapon individually, make a single offset here for each character")]
        public Vector3 ikRotationOffsetR;
        [Tooltip("Instead of adjust each weapon individually, make a single offset here for each character")]
        public Vector3 ikPositionOffsetR;
        [Tooltip("Instead of adjust each weapon individually, make a single offset here for each character")]
        public Vector3 ikRotationOffsetL;
        [Tooltip("Instead of adjust each weapon individually, make a single offset here for each character")]
        public Vector3 ikPositionOffsetL;
        [vEditorToolbar("Ammo UI")]
        [Tooltip("Use the vAmmoDisplay to shot ammo count")]
        public bool useAmmoDisplay = true;
        [Tooltip("ID to find ammoDisplay for leftWeapon")]
        public int leftWeaponAmmoDisplayID = -1;
        [Tooltip("ID to find ammoDisplay for rightWeapon")]
        public int rightWeaponAmmoDisplayID = 1;

        [vEditorToolbar("LockOn")]
        [Header("- LockOn (need the shooter lockon component)")]
        [Tooltip("Allow the use of the LockOn or not")]
        public bool useLockOn = false;
        [Tooltip("Allow the use of the LockOn only with a Melee Weapon")]
        public bool useLockOnMeleeOnly = true;

        [vEditorToolbar("HipFire")]
        [Header("- HipFire Options")]
        [Tooltip("If enable, remember to change your weak attack input to other input - this allows shot without aim")]
        public bool hipfireShot = false;
        [Tooltip("Precision of the weapon when shooting using hipfire (without aiming)")]
        public float hipfireDispersion = 0.5f;

        [vEditorToolbar("Camera Sway")]
        [Header("- Camera Sway Settings")]
        [Tooltip("Camera Sway movement while aiming")]
        public float cameraMaxSwayAmount = 2f;
        [Tooltip("Camera Sway Speed while aiming")]
        public float cameraSwaySpeed = .5f;

        [vEditorToolbar("Weapons")]
        public vShooterWeapon rWeapon, lWeapon;
        [HideInInspector]
        public vAmmoManager ammoManager;
        public TotalAmmoHandler totalAmmoHandler;

        [HideInInspector]
        public OnReloadWeapon onReloadWeapon;
        [HideInInspector]
        public vAmmoDisplay ammoDisplayR, ammoDisplayL;
        [HideInInspector]
        public vCamera.vThirdPersonCamera tpCamera;
        [HideInInspector]
        public bool showCheckAimGizmos;

        private Animator animator;
        private int totalAmmo;
        private int secundaryTotalAmmo;
        private bool usingThirdPersonController;
        private float currentShotTime;
        private float hipfirePrecisionAngle;
        private float hipfirePrecision;
        #endregion        

        void Start()
        {
            animator = GetComponent<Animator>();
            if (applyRecoilToCamera)
                tpCamera = FindObjectOfType<vCamera.vThirdPersonCamera>();
            
            ammoManager = GetComponent<vAmmoManager>();
            ammoManager.updateTotalAmmo = new vAmmoManager.OnUpdateTotalAmmo(UpdateTotalAmmo);
            usingThirdPersonController = GetComponent<vThirdPersonController>() != null;
            if (useAmmoDisplay)
            {
                GetAmmoDisplays();
            }

            if (animator)
            {
                var _rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var _lefttHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                var weaponR = _rightHand.GetComponentInChildren<vShooterWeapon>();
                var weaponL = _lefttHand.GetComponentInChildren<vShooterWeapon>();
                if (weaponR != null)
                    SetRightWeapon(weaponR.gameObject);
                if (weaponL != null)
                    SetLeftWeapon(weaponL.gameObject);
            }

            if (!ignoreTags.Contains(gameObject.tag))
                ignoreTags.Add(gameObject.tag);
            if (useAmmoDisplay)
            {
                if (ammoDisplayR) ammoDisplayR.UpdateDisplay("");
                if (ammoDisplayL) ammoDisplayL.UpdateDisplay("");
            }
            UpdateTotalAmmo();
        }

        public void SetLeftWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponent<vShooterWeapon>();
                lWeapon = w;
                if (lWeapon)
                {
                    lWeapon.ignoreTags = ignoreTags;
                    lWeapon.hitLayer = damageLayer;
                    lWeapon.root = transform;
                    lWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (lWeapon.autoReload) ReloadWeaponAuto(lWeapon, false, false);
                    if (lWeapon.secundaryWeapon)
                    {
                        lWeapon.secundaryWeapon.ignoreTags = ignoreTags;
                        lWeapon.secundaryWeapon.hitLayer = damageLayer;
                        lWeapon.secundaryWeapon.root = transform;
                        lWeapon.secundaryWeapon.isSecundaryWeapon = true;
                        if (lWeapon.secundaryWeapon.autoReload) ReloadWeaponAuto(lWeapon.secundaryWeapon, false, true);
                    }
                    if (usingThirdPersonController)
                    {
                        if (useAmmoDisplay && !ammoDisplayL) GetAmmoDisplays();
                        if (useAmmoDisplay && ammoDisplayL) ammoDisplayL.Show();
                        UpdateLeftAmmo();
                    }
                    currentShotTime = 0;
                }
            }
        }

        public void SetRightWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponent<vShooterWeapon>();
                rWeapon = w;
                if (rWeapon)
                {
                    rWeapon.ignoreTags = ignoreTags;
                    rWeapon.hitLayer = damageLayer;
                    rWeapon.root = transform;
                    rWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (rWeapon.autoReload) ReloadWeaponAuto(rWeapon, false, false);
                    if (rWeapon.secundaryWeapon)
                    {
                        rWeapon.secundaryWeapon.ignoreTags = ignoreTags;
                        rWeapon.secundaryWeapon.hitLayer = damageLayer;
                        rWeapon.secundaryWeapon.root = transform;
                        rWeapon.secundaryWeapon.isSecundaryWeapon = true;
                        if (rWeapon.secundaryWeapon.autoReload) ReloadWeaponAuto(rWeapon.secundaryWeapon, false, true);
                    }
                    if (usingThirdPersonController)
                    {
                        if (useAmmoDisplay && !ammoDisplayR) GetAmmoDisplays();
                        if (useAmmoDisplay && ammoDisplayR) ammoDisplayR.Show();
                        UpdateRightAmmo();
                    }
                    currentShotTime = 0;
                }
            }
        }

        public void OnDestroyWeapon(GameObject otherGameObject)
        {
            if (usingThirdPersonController)
            {
                var ammoDisplay = rWeapon != null && otherGameObject == rWeapon.gameObject ? ammoDisplayR : lWeapon != null && otherGameObject == lWeapon.gameObject ? ammoDisplayL : null;

                if (useAmmoDisplay && ammoDisplay)
                {
                    ammoDisplay.UpdateDisplay("");
                    ammoDisplay.Hide();
                }
            }
            currentShotTime = 0;
        }

        protected void GetAmmoDisplays()
        {
            var ammoDisplays = FindObjectsOfType<vAmmoDisplay>();
            if (ammoDisplays.Length > 0)
            {
                if (!ammoDisplayL)
                    ammoDisplayL = ammoDisplays.vToList().Find(d => d.displayID == leftWeaponAmmoDisplayID);
                if (!ammoDisplayR)
                    ammoDisplayR = ammoDisplays.vToList().Find(d => d.displayID == rightWeaponAmmoDisplayID);
            }
        }

        public int GetMoveSetID()
        {
            int id = 0;
            if (rWeapon) id = (int)rWeapon.moveSetID;
            else if (lWeapon) id = (int)lWeapon.moveSetID;
            return id;
        }

        public int GetUpperBodyID()
        {
            int id = 0;
            if (rWeapon) id = (int)rWeapon.upperBodyID;
            else if (lWeapon) id = (int)lWeapon.upperBodyID;
            return id;
        }

        public int GetAttackID()
        {
            int id = 0;
            if (rWeapon) id = (int)rWeapon.attackID;
            else if (lWeapon) id = (int)lWeapon.attackID;
            return id;
        }

        public int GetEquipID()
        {
            int id = 0;
            if (rWeapon) id = (int)rWeapon.equipID;
            else if (lWeapon) id = (int)lWeapon.equipID;
            return id;
        }

        public int GetReloadID()
        {
            int id = 0;
            if (rWeapon) id = (int)rWeapon.reloadID;
            else if (lWeapon) id = (int)lWeapon.reloadID;
            return id;
        }

        public virtual bool WeaponHasAmmo(bool secundaryWeapon = false)
        {
            return secundaryWeapon ? secundaryTotalAmmo > 0 : totalAmmo > 0;
        }

        public bool isShooting
        {
            get { return currentShotTime > 0; }
        }

        public void ReloadWeapon(bool ignoreAmmo = false, bool ignoreAnim = false)
        {
            var weapon = rWeapon ? rWeapon : lWeapon;

            if (!weapon) return;
            UpdateTotalAmmo();
            bool primaryWeaponAnim = false;
            if (!(!ignoreAmmo && (weapon.ammoCount >= weapon.clipSize || !WeaponHasAmmo())) && !weapon.autoReload)
            {
                if (!ignoreAnim)
                    onReloadWeapon.Invoke(weapon);
                var needAmmo = weapon.clipSize - weapon.ammoCount;
                if (!ignoreAmmo && WeaponAmmo(weapon).count < needAmmo)
                    needAmmo = WeaponAmmo(weapon).count;

                weapon.AddAmmo(needAmmo);
                if (!ignoreAmmo)
                    WeaponAmmo(weapon).Use(needAmmo);
                if (animator && !ignoreAnim)
                {
                    animator.SetInteger("ReloadID", GetReloadID());
                    animator.SetTrigger("Reload");
                }
                if (!ignoreAnim)
                    weapon.ReloadEffect();
                primaryWeaponAnim = true;
            }
            if (weapon.secundaryWeapon && !((weapon.secundaryWeapon.ammoCount >= weapon.secundaryWeapon.clipSize || !WeaponHasAmmo(true))) && !weapon.secundaryWeapon.autoReload)
            {
                var needAmmo = weapon.secundaryWeapon.clipSize - weapon.secundaryWeapon.ammoCount;
                if (!ignoreAmmo && WeaponAmmo(weapon.secundaryWeapon).count < needAmmo)
                    needAmmo = WeaponAmmo(weapon.secundaryWeapon).count;
                weapon.secundaryWeapon.AddAmmo(needAmmo);
                if (!ignoreAmmo)
                    WeaponAmmo(weapon.secundaryWeapon).Use(needAmmo);
                if (!primaryWeaponAnim)
                {
                    if (animator && !ignoreAnim)
                    {
                        primaryWeaponAnim = true;
                        animator.SetInteger("ReloadID", weapon.secundaryWeapon.reloadID);
                        animator.SetTrigger("Reload");
                    }
                    if (!ignoreAnim)
                        weapon.secundaryWeapon.ReloadEffect();
                }
            }
            UpdateTotalAmmo();
        }

        protected void ReloadWeaponAuto(vShooterWeapon weapon, bool ignoreAmmo, bool secundaryWeapon = false)
        {
            if (!weapon) return;
            UpdateTotalAmmo();

            if (!(!ignoreAmmo && (weapon.ammoCount >= weapon.clipSize || !WeaponHasAmmo(secundaryWeapon))))
            {
                var needAmmo = weapon.clipSize - weapon.ammoCount;

                if (!ignoreAmmo && WeaponAmmo(weapon).count < needAmmo)
                    needAmmo = WeaponAmmo(weapon).count;

                weapon.AddAmmo(needAmmo);
                if (!ignoreAmmo)
                    WeaponAmmo(weapon).Use(needAmmo);
            }
        }

        public vAmmo WeaponAmmo(vShooterWeapon weapon)
        {
            if (!weapon) return null;
            var ammo = new vAmmo();
            if (ammoManager && ammoManager.ammos != null && ammoManager.ammos.Count > 0)
            {
                ammo = ammoManager.GetAmmo(weapon.ammoID);
            }
            return ammo;
        }

        public void UpdateTotalAmmo()
        {
            UpdateLeftAmmo();
            UpdateRightAmmo();
        }

        public void UpdateLeftAmmo()
        {
            if (!lWeapon) return;
            UpdateTotalAmmo(lWeapon, ref totalAmmo, -1);
            UpdateTotalAmmo(lWeapon.secundaryWeapon, ref secundaryTotalAmmo, -1);
        }

        public void UpdateRightAmmo()
        {
            if (!rWeapon) return;
            UpdateTotalAmmo(rWeapon, ref totalAmmo, 1);
            UpdateTotalAmmo(rWeapon.secundaryWeapon, ref secundaryTotalAmmo, 1);
        }

        protected virtual void UpdateTotalAmmo(vShooterWeapon weapon, ref int targetTotalAmmo, int displayId)
        {
            if (!weapon) return;
            var ammoCount = 0;
            var ammo = WeaponAmmo(weapon);
            if (ammo != null) ammoCount += ammo.count;

            targetTotalAmmo = ammoCount;
            UpdateAmmoDisplay(displayId);
        }

        protected virtual void UpdateAmmoDisplay(int displayId)
        {
            if (!useAmmoDisplay) return;
            var weapon = displayId == 1 ? rWeapon : lWeapon;

            if (!weapon) return;
            if (!ammoDisplayR || !ammoDisplayL) GetAmmoDisplays();
            var ammoDisplay = displayId == 1 ? ammoDisplayR : ammoDisplayL;
            if (useAmmoDisplay && ammoDisplay)
            {
                if (weapon.secundaryWeapon)
                {
                    var display1 = "A: " + (weapon.autoReload ? (weapon.ammoCount + totalAmmo).ToString() : (string.Format("{0} / {1}", weapon.ammoCount, totalAmmo)));
                    var displat2 = "B: " + (weapon.secundaryWeapon.autoReload ? (weapon.secundaryWeapon.ammoCount + secundaryTotalAmmo).ToString() : (string.Format("{0} / {1}", weapon.secundaryWeapon.ammoCount, secundaryTotalAmmo)));
                    ammoDisplay.UpdateDisplay(display1 + "\n" + displat2, weapon.ammoID);
                }
                else
                    ammoDisplay.UpdateDisplay(weapon.autoReload ? (weapon.ammoCount + totalAmmo).ToString() : (string.Format("{0} / {1}", weapon.ammoCount, totalAmmo)), weapon.ammoID);
            }
        }

        public virtual void Shoot(Vector3 aimPosition, bool applyHipfirePrecision = false, bool useSecundaryWeapon = false)
        {
            if (isShooting) return;
            var weapon = rWeapon ? rWeapon : lWeapon;
            if (!weapon) return;
            var secundaryWeapon = weapon.secundaryWeapon;

            if (useSecundaryWeapon && !secundaryWeapon)
            {
                return;
            }
            var targetWeapon = useSecundaryWeapon ? secundaryWeapon : weapon;
            if (targetWeapon.autoReload) ReloadWeaponAuto(targetWeapon, false, useSecundaryWeapon);
            if (targetWeapon.ammoCount > 0)
            {
                var _aimPos = applyHipfirePrecision ? aimPosition + HipFirePrecision(aimPosition) : aimPosition;
                targetWeapon.ShootEffect(_aimPos, transform);

                if (applyRecoilToCamera)
                {
                    var recoilHorizontal = Random.Range(targetWeapon.recoilLeft, targetWeapon.recoilRight);
                    var recoilUp = Random.Range(0, targetWeapon.recoilUp);
                    StartCoroutine(Recoil(recoilHorizontal, recoilUp));
                }

                UpdateAmmoDisplay(rWeapon ? 1 : -1);
            }
            else
            {
                weapon.EmptyClipEffect();
            }
            if (targetWeapon.autoReload) ReloadWeaponAuto(targetWeapon, false, useSecundaryWeapon);
            currentShotTime = weapon.shootFrequency;
        }

        IEnumerator Recoil(float horizontal, float up)
        {
            yield return new WaitForSeconds(0.02f);
            if (animator) animator.SetTrigger("Shoot");
            if (tpCamera != null) tpCamera.RotateCamera(horizontal, up);
        }

        protected virtual Vector3 HipFirePrecision(Vector3 _aimPosition)
        {
            var weapon = rWeapon ? rWeapon : lWeapon;
            if (!weapon) return Vector3.zero;
            hipfirePrecisionAngle = UnityEngine.Random.Range(-1000, 1000);
            hipfirePrecision = Random.Range(-hipfireDispersion, hipfireDispersion);
            var dir = (Quaternion.AngleAxis(hipfirePrecisionAngle, _aimPosition - weapon.muzzle.position) * (Vector3.up)).normalized * hipfirePrecision;
            return dir;
        }

        public void CameraSway()
        {
            var weapon = rWeapon ? rWeapon : lWeapon;
            if (!weapon) return;
            float bx = (Mathf.PerlinNoise(0, Time.time * cameraSwaySpeed) - 0.5f);
            float by = (Mathf.PerlinNoise(0, (Time.time * cameraSwaySpeed) + 100)) - 0.5f;

            var swayAmount = cameraMaxSwayAmount * (1f - weapon.precision);
            if (swayAmount == 0) return;

            bx *= swayAmount;
            by *= swayAmount;

            float tx = (Mathf.PerlinNoise(0, Time.time * cameraSwaySpeed) - 0.5f);
            float ty = ((Mathf.PerlinNoise(0, (Time.time * cameraSwaySpeed) + 100)) - 0.5f);

            tx *= -(swayAmount * 0.25f);
            ty *= (swayAmount * 0.25f);

            if (tpCamera != null)
            {
                vCamera.vThirdPersonCamera.instance.offsetMouse.x = bx + tx;
                vCamera.vThirdPersonCamera.instance.offsetMouse.y = by + ty;
            }            
        }

        public void UpdateShotTime()
        {
            if (currentShotTime > 0) currentShotTime -= Time.deltaTime;
        }
    }
}