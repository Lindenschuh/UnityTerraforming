using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Invector.vCharacterController
{
    [RequireComponent(typeof(LineRenderer))]
    [vClassHeader("THROW OBJECT")]
    public class vThrowObject : vMonoBehaviour
    {
        #region public variables
        public enum CameraStyle
        {
            ThirdPerson, TopDown, SideScroll
        }

        public CameraStyle cameraStyle;
        public GenericInput throwInput = new GenericInput("Mouse0", "RB", "RB");
        public GenericInput aimThrowInput = new GenericInput("G", "LB", "LB");
        public Transform throwStartPoint;
        public GameObject throwEnd;
        public Rigidbody objectToThrow;
        public LayerMask obstacles = 1 << 0;
        public float throwMaxForce = 15f;
        public float throwDelayTime = .25f;
        public float lineStepPerTime = .1f;
        public float lineMaxTime = 10f;
        public float exitStrafeModeDelay = 0.5f;
        public string throwAnimation = "ThrowObject";
        public string holdingAnimation = "HoldingObject";
        public string cancelAnimation = "CancelThrow";
        public int maxThrowObjects = 6;
        public int currentThrowObject;
        public bool debug;
        public UnityEngine.Events.UnityEvent onEnableAim;
        public UnityEngine.Events.UnityEvent onCancelAim;
        public UnityEngine.Events.UnityEvent onThrowObject;
        public UnityEngine.Events.UnityEvent onCollectObject;

        #endregion

        #region private variables

        private int vertexCount;
        private bool isDrawing;
        private bool isAiming;
        private bool inThrow;
        private bool isThrowInput;
        private Transform rightUpperArm;
        private LineRenderer lineRenderer;
        private Quaternion upperArmRotation;
        private Animator animator;

        vThirdPersonInput tpInput;
        RaycastHit hit;

        #endregion

        #region Protected methods

        void Start()
        {
            tpInput = GetComponentInParent<vThirdPersonInput>();
            this.lineRenderer = GetComponent<LineRenderer>();
            animator = GetComponentInParent<Animator>();
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        }

        void Update()
        {
            if (isAiming)
            {
                var dir = aimDirection;
                dir.y = 0;
                if (cameraStyle != CameraStyle.SideScroll)
                    tpInput.cc.RotateToDirection(dir);
                else
                {
                    var angle = (Quaternion.LookRotation(aimDirection).eulerAngles - transform.rotation.eulerAngles).NormalizeAngle();
                    if (angle.y > 150 || angle.y < -150)
                        tpInput.cc.RotateToDirection(-transform.forward, true);
                }
            }
        }

        void LateUpdate()
        {
            if (objectToThrow == null || !tpInput.enabled || tpInput.cc.customAction)
            {
                isAiming = false;
                inThrow = false;
                isThrowInput = false;
                return;
            }

            UpdateThrow();
            UpdateInput();
        }

        void UpdateInput()
        {
            if (aimThrowInput.GetButtonDown() && !isAiming && !inThrow)
            {
                isAiming = true;
                tpInput.cc.lockInStrafe = true;
                animator.CrossFadeInFixedTime(holdingAnimation, 0.2f);
                onEnableAim.Invoke();
            }
            if (aimThrowInput.GetButtonUp())
            {
                isAiming = false;
                tpInput.cc.lockInStrafe = false;
                animator.CrossFadeInFixedTime(cancelAnimation, 0.2f);
                onCancelAim.Invoke();
            }
            if (throwInput.GetButtonDown() && isAiming && !inThrow)
            {
                isAiming = false;
                isThrowInput = true;
            }
        }

        void LaunchObject(Rigidbody projectily)
        {
            projectily.AddForce(StartVelocity, ForceMode.VelocityChange);
        }

        void UpdateThrow()
        {
            if (objectToThrow == null || !tpInput.enabled || tpInput.cc.customAction)
            {
                isAiming = false;
                inThrow = false;
                isThrowInput = false;
                if (lineRenderer && lineRenderer.enabled) lineRenderer.enabled = false;
                if (throwEnd && throwEnd.activeSelf) throwEnd.SetActive(false);
                return;
            }

            if (isAiming)
                DrawTrajectory();
            else
            {
                if (lineRenderer && lineRenderer.enabled) lineRenderer.enabled = false;
                if (throwEnd && throwEnd.activeSelf) throwEnd.SetActive(false);
            }

            if (isThrowInput)
            {
                inThrow = true;
                isThrowInput = false;
                animator.CrossFadeInFixedTime(throwAnimation, 0.2f);
                currentThrowObject -= 1;
                StartCoroutine(Launch());
            }
        }

        void DrawTrajectory()
        {
            var points = GetTrajectoryPoints(throwStartPoint.position, StartVelocity, lineStepPerTime, lineMaxTime);
            if (lineRenderer)
            {
                if (!lineRenderer.enabled) lineRenderer.enabled = true;
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
            if (throwEnd)
            {
                if (!throwEnd.activeSelf) throwEnd.SetActive(true);
                if (points.Count > 1)
                    throwEnd.transform.position = points[points.Count - 1];
            }
        }

        IEnumerator Launch()
        {
            yield return new WaitForSeconds(throwDelayTime);
            var obj = Instantiate(objectToThrow, throwStartPoint.position, throwStartPoint.rotation) as Rigidbody;
            obj.isKinematic = false;
            LaunchObject(obj);
            onThrowObject.Invoke();

            yield return new WaitForSeconds(2 * lineStepPerTime);
            var coll = obj.GetComponent<Collider>();
            if (coll)
                coll.isTrigger = false;

            inThrow = false;

            if (currentThrowObject <= 0)
                objectToThrow = null;
            yield return new WaitForSeconds(exitStrafeModeDelay);
            //onCancelAim.Invoke();
            tpInput.cc.lockInStrafe = false;
        }

        Vector3 thirdPersonAimPoint
        {
            get
            {
                return throwStartPoint.position + Camera.main.transform.forward * throwMaxForce;
            }
        }

        Vector3 topdownAimPoint
        {
            get
            {
                var pos = vMousePositionHandler.Instance.WorldMousePosition(obstacles);
                pos.y = transform.position.y;
                return pos;
            }
        }

        Vector3 sideScrollAimPoint
        {
            get
            {
                var pos = transform.InverseTransformPoint(vMousePositionHandler.Instance.WorldMousePosition(obstacles));
                pos.x = 0;
                return transform.TransformPoint(pos);
            }
        }

        Vector3 StartVelocity
        {
            get
            {
                RaycastHit hit;
                var dist = Vector3.Distance(transform.position, aimPoint);
                Debug.DrawLine(transform.position, aimPoint);
                if (cameraStyle == CameraStyle.ThirdPerson)
                    if (Physics.Raycast(throwStartPoint.position, aimDirection.normalized, out hit, obstacles)) dist = hit.distance;
                if (cameraStyle != CameraStyle.SideScroll)
                {
                    var force = Mathf.Clamp(dist, 0, throwMaxForce);
                    var rotation = Quaternion.LookRotation(aimDirection.normalized, Vector3.up);
                    var dir = Quaternion.AngleAxis(rotation.eulerAngles.NormalizeAngle().x, transform.right) * transform.forward;
                    return dir * force;
                }
                else
                {
                    var force = Mathf.Clamp(dist, 0, throwMaxForce);
                    return aimDirection.normalized * force;
                }
            }
        }

        Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
        {
            return start + startVelocity * time + Physics.gravity * time * time * 0.5f;
        }

        List<Vector3> GetTrajectoryPoints(Vector3 start, Vector3 startVelocity, float timestep, float maxTime)
        {
            Vector3 prev = start;
            List<Vector3> points = new List<Vector3>();
            points.Add(prev);
            for (int i = 1; ; i++)
            {
                float t = timestep * i;
                if (t > maxTime) break;
                Vector3 pos = PlotTrajectoryAtTime(start, startVelocity, t);
                RaycastHit hit;
                if (Physics.Linecast(prev, pos, out hit, obstacles))
                {
                    points.Add(hit.point);
                    break;
                }
                if (debug) Debug.DrawLine(prev, pos, Color.red);
                points.Add(pos);
                prev = pos;

            }
            return points;
        }

        #endregion

        #region Public Methods
        public virtual Vector3 aimPoint
        {
            get
            {
                switch (cameraStyle)
                {
                    case CameraStyle.ThirdPerson: return thirdPersonAimPoint;
                    case CameraStyle.TopDown: return topdownAimPoint;
                    case CameraStyle.SideScroll: return sideScrollAimPoint;
                }
                return throwStartPoint.position + Camera.main.transform.forward * throwMaxForce;
            }
        }

        public virtual Vector3 aimDirection
        {
            get
            {
                return aimPoint - rightUpperArm.position;
            }
        }

        public virtual void SetAmount(int value)
        {
            currentThrowObject += value;
            onCollectObject.Invoke();
        }
        #endregion
    }
}