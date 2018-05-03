using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController
{
    [vClassHeader("Input Manager", iconName = "inputIcon")]
    public class vThirdPersonInput : vMonoBehaviour
    {

        #region Variables        

        [vEditorToolbar("Inputs")]
        [Header("Default Input")]
        public bool lockInput;
        [Header("Uncheck if you need to use the cursor")]
        public bool unlockCursorOnStart = false;
        public bool showCursorOnStart = false;
        public GenericInput horizontalInput = new GenericInput("Horizontal", "LeftAnalogHorizontal", "Horizontal");
        public GenericInput verticallInput = new GenericInput("Vertical", "LeftAnalogVertical", "Vertical");
        public GenericInput jumpInput = new GenericInput("Space", "X", "X");
        public GenericInput rollInput = new GenericInput("Q", "B", "B");
        public GenericInput strafeInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
        public GenericInput sprintInput = new GenericInput("LeftShift", "LeftStickClick", "LeftStickClick");
        public GenericInput crouchInput = new GenericInput("C", "Y", "Y");

        [vEditorToolbar("Camera Settings")]
        public bool lockCameraInput;
        public bool ignoreCameraRotation;
        public bool rotateToCameraWhileStrafe = true;

        [vEditorToolbar("Inputs")]
        [Header("Camera Input")]
        public GenericInput rotateCameraXInput = new GenericInput("Mouse X", "RightAnalogHorizontal", "Mouse X");
        public GenericInput rotateCameraYInput = new GenericInput("Mouse Y", "RightAnalogVertical", "Mouse Y");
        public GenericInput cameraZoomInput = new GenericInput("Mouse ScrollWheel", "", "");
        [HideInInspector]
        public vCamera.vThirdPersonCamera tpCamera;              // acess camera info                
        [HideInInspector]
        public string customCameraState;                    // generic string to change the CameraState        
        [HideInInspector]
        public string customlookAtPoint;                    // generic string to change the CameraPoint of the Fixed Point Mode        
        [HideInInspector]
        public bool changeCameraState;                      // generic bool to change the CameraState        
        [HideInInspector]
        public bool smoothCameraState;                      // generic bool to know if the state will change with or without lerp  
        [HideInInspector]
        public bool keepDirection;                          // keep the current direction in case you change the cameraState
        protected Vector2 oldInput;
        [vEditorToolbar("Events")]
        public UnityEngine.Events.UnityEvent OnLateUpdate;

        [HideInInspector]
        public vThirdPersonController cc;                   // access the ThirdPersonController component
        [HideInInspector]
        public vHUDController hud;                          // acess vHUDController component        
        protected bool updateIK = false;
        protected bool isInit;

        protected InputDevice inputDevice { get { return vInput.instance.inputDevice; } }
        public Animator animator
        {
            get
            {
                if (cc == null) cc = GetComponent<vThirdPersonController>();
                if (cc.animator == null) return GetComponent<Animator>();
                return cc.animator;
            }
        }

        #endregion

        #region Initialize Character, Camera & HUD when LoadScene

        protected virtual void Start()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();

            if (vThirdPersonController.instance == cc || vThirdPersonController.instance == null)
            {
                StartCoroutine(CharacterInit());
            }

            ShowCursor(showCursorOnStart);
            LockCursor(unlockCursorOnStart);
        }       

        protected virtual IEnumerator CharacterInit()
        {
            yield return new WaitForEndOfFrame();
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vCamera.vThirdPersonCamera>();
                if (tpCamera && tpCamera.target != transform) tpCamera.SetMainTarget(this.transform);
            }
            if (hud == null && vHUDController.instance != null)
            {
                hud = vHUDController.instance;
                hud.Init(cc);
            }
        }

        #endregion

        protected virtual void LateUpdate()
        {
            if (cc == null || Time.timeScale == 0) return;
            if ((!updateIK && animator.updateMode == AnimatorUpdateMode.AnimatePhysics)) return;

            CameraInput();                      // update camera input
            UpdateCameraStates();               // update camera states
            OnLateUpdate.Invoke();
            updateIK = false;
        }

        protected virtual void FixedUpdate()
        {
            cc.ControlLocomotion();
            cc.AirControl();                    // update air behaviour            
            updateIK = true;
        }

        protected virtual void Update()
        {
            if (cc == null || Time.timeScale == 0) return;

            InputHandle();                      // update input methods
            cc.UpdateMotor();                   // call ThirdPersonMotor methods
            //cc.UpdateAnimator();                // call ThirdPersonAnimator methods
            UpdateHUD();                        // update hud graphics
        }

        protected virtual void InputHandle()
        {
            if (lockInput || cc.lockMovement || cc.ragdolled) return;

            MoveCharacter();
            SprintInput();
            CrouchInput();
            StrafeInput();
            JumpInput();
            RollInput();
        }

        #region Generic Methods
        // you can use these methods with Playmaker or AdventureCreator to have better control on cutscenes and events.

        /// <summary>
        /// Lock all the Input from the Player
        /// </summary>
        /// <param name="value"></param>
        public void SetLockBasicInput(bool value)
        {
            lockInput = value;
            if (value)
            {
                cc.input = Vector2.zero;
                cc.isSprinting = false;
                cc.animator.SetFloat("InputHorizontal", 0, 0.25f, Time.deltaTime);
                cc.animator.SetFloat("InputVertical", 0, 0.25f, Time.deltaTime);
                cc.animator.SetFloat("InputMagnitude", 0, 0.25f, Time.deltaTime);
            }
        }

        /// <summary>
        /// Show/Hide Cursor
        /// </summary>
        /// <param name="value"></param>
        public void ShowCursor(bool value)
        {
            Cursor.visible = value;
        }

        /// <summary>
        /// Lock/Unlock the cursor to the center of screen
        /// </summary>
        /// <param name="value"></param>
        public void LockCursor(bool value)
        {
            if (!value)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Lock the Camera Input
        /// </summary>
        /// <param name="value"></param>
        public void SetLockCameraInput(bool value)
        {
            lockCameraInput = value;
        }

        /// <summary>
        /// If you're using the MoveCharacter method with a custom targetDirection, check this true to align the character with your custom targetDirection
        /// </summary>
        /// <param name="value"></param>
        public void IgnoreCameraRotation(bool value)
        {
            ignoreCameraRotation = value;
        }

        /// <summary>
        /// Limits the character to walk only, useful for cutscenes and 'indoor' areas
        /// </summary>
        /// <param name="value"></param>
        public void SetWalkByDefault(bool value)
        {
            cc.freeSpeed.walkByDefault = value;
            cc.strafeSpeed.walkByDefault = value;
        }

        #endregion

        #region Basic Locomotion Inputs      

        public virtual void MoveCharacter(Vector3 position, bool rotateToDirection = true)
        {
            var dir = position - transform.position;
            var targetDir = cc.isStrafing ? transform.InverseTransformDirection(dir).normalized : dir.normalized;
            cc.input.x = targetDir.x;
            cc.input.y = targetDir.z;

            if (!keepDirection)
                oldInput = cc.input;

            if (rotateToDirection && cc.isStrafing)
            {
                targetDir.y = 0;
                cc.RotateToDirection(dir);
                Debug.DrawRay(transform.position, dir * 10f, Color.blue);
            }
            else if (rotateToDirection)
            {
                targetDir.y = 0;
                cc.targetDirection = targetDir;
            }
        }

        public virtual void MoveCharacter(Transform _transform, bool rotateToDirection = true)
        {
            MoveCharacter(_transform.position, rotateToDirection);
        }

        protected virtual void MoveCharacter()
        {
            // gets input from mobile           
            cc.input.x = horizontalInput.GetAxis();
            cc.input.y = verticallInput.GetAxis();
            // update oldInput to compare with current Input if keepDirection is true
            if (!keepDirection)
                oldInput = cc.input;
        }

        protected virtual void StrafeInput()
        {
            if (strafeInput.GetButtonDown())
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (sprintInput.GetButtonDown())
                cc.Sprint(true);
            else
                cc.Sprint(false);
        }

        protected virtual void CrouchInput()
        {
            if (crouchInput.GetButtonDown())
                cc.Crouch();
        }

        protected virtual void JumpInput()
        {
            if (jumpInput.GetButtonDown())
                cc.Jump(true);
        }

        protected virtual void RollInput()
        {
            if (rollInput.GetButtonDown())
                cc.Roll();
        }
        #endregion

        #region Camera Methods

        public virtual void CameraInput()
        {
            if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
            if (!ignoreCameraRotation)
            {
                if (!keepDirection) cc.UpdateTargetDirection(Camera.main.transform);
                RotateWithCamera(Camera.main.transform);
            }

            if (tpCamera == null)
                return;

            var Y = lockCameraInput ? 0f : rotateCameraYInput.GetAxis();
            var X = lockCameraInput ? 0f : rotateCameraXInput.GetAxis();
            var zoom = cameraZoomInput.GetAxis();

            tpCamera.RotateCamera(X, Y);
            tpCamera.Zoom(zoom);

            // change keedDirection from input diference
            if (keepDirection && Vector2.Distance(cc.input, oldInput) > 0.2f) keepDirection = false;
        }

        protected virtual void UpdateCameraStates()
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
                tpCamera.ChangeState(customCameraState, customlookAtPoint, smoothCameraState);
            else if (cc.isCrouching)
                tpCamera.ChangeState("Crouch", true);
            else if (cc.isStrafing)
                tpCamera.ChangeState("Strafing", true);
            else
                tpCamera.ChangeState("Default", true);
        }

        public void ChangeCameraState(string cameraState)
        {
            changeCameraState = true;
            customCameraState = cameraState;
        }

        public void ResetCameraState()
        {
            changeCameraState = false;
            customCameraState = string.Empty;
        }

        protected virtual void RotateWithCamera(Transform cameraTransform)
        {
            if (rotateToCameraWhileStrafe && cc.isStrafing && !cc.actions && !cc.lockMovement)
            {
                // smooth align character with aim position               
                if (tpCamera != null && tpCamera.lockTarget)
                {
                    cc.RotateToTarget(tpCamera.lockTarget);
                }
                // rotate the camera around the character and align with when the char move
                else if (cc.input != Vector2.zero)
                {
                    cc.RotateWithAnotherTransform(cameraTransform);
                }
            }
        }

        #endregion

        #region HUD       

        public virtual void UpdateHUD()
        {
            if (hud == null)
                return;

            hud.UpdateHUD(cc);
        }

        #endregion
    }
}