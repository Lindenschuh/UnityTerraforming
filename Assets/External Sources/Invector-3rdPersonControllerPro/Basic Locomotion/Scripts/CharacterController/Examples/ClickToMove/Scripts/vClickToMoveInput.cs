using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.vCharacterController.ClickToMove
{
    [vClassHeader("Click To Move Input")]
    public class vClickToMoveInput : vThirdPersonInput
    {
        #region Variables 

        [System.Serializable]
        public class vCursorByTag
        {
            public string tag;
            public Texture2D cursorTexture;
            public CursorMode cursorMode = CursorMode.Auto;
        }

        [vEditorToolbar("Cursor")]
        public List<vCursorByTag> cursorByTag;

        [vEditorToolbar("Layer")]
        [Header("Click To Move Properties")]
        public LayerMask clickMoveLayer = 1 << 0;
                
        [System.Serializable]
        public class  vOnEnableCursor: UnityEngine.Events.UnityEvent<Vector3>{}
        [vEditorToolbar("Events")]
        public vOnEnableCursor onEnableCursor = new vOnEnableCursor();
        public UnityEngine.Events.UnityEvent onDisableCursor;

        [HideInInspector]
        public Vector3 cursorPoint;        
        public Collider target { get; set; }
        public Dictionary<string, vCursorByTag> customCursor;

        #endregion

        protected override void Start()
        {
            base.Start();
            customCursor = new Dictionary<string, vCursorByTag>();

            for (int i = 0; i < cursorByTag.Count; i++)
            {
                if(!customCursor.ContainsKey(cursorByTag[i].tag))
                {
                    customCursor.Add(cursorByTag[i].tag, cursorByTag[i]);
                }
            }
        }

        protected override IEnumerator CharacterInit()
        {
           yield return StartCoroutine(base.CharacterInit());
           cursorPoint = transform.position;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            MoveToPoint();
        }

        protected override void MoveCharacter()
        {
            cc.rotateByWorld = true;
            ClickAndMove();
        }

        protected virtual void ClickAndMove()
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clickMoveLayer))
            {
                var tag = hit.collider.gameObject.tag;
                ChangeCursorByTag(tag);
                CheckClickPoint(hit);
            }
        }

        protected virtual void CheckClickPoint(RaycastHit hit)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    target = hit.collider;
                }

                if (onEnableCursor != null)
                {
                    onEnableCursor.Invoke(hit.point);
                }
                cursorPoint = hit.point;
            }
        }

        protected virtual void ChangeCursorByTag(string tag)
        {
            if (customCursor.Count <= 0) return;

            if (customCursor.ContainsKey(tag))
            {
                Cursor.SetCursor(customCursor[tag].cursorTexture, Vector2.zero, customCursor[tag].cursorMode);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        protected void MoveToPoint()
        {
            if (!NearPoint(cursorPoint, transform.position) && target)
                MoveCharacter(cursorPoint);
            else
            {
                if (onDisableCursor != null)
                    onDisableCursor.Invoke();

                cc.input = Vector2.Lerp(cc.input, Vector3.zero, 20 * Time.deltaTime);
            }
        }

        public void SetTargetPosition(Vector3 value)
        {
            cursorPoint = value;
            var dir = (value - transform.position).normalized;
            cc.input = new Vector2(dir.x, dir.z);
        }

        public void ClearTarget()
        {
            cc.input = Vector2.zero;
            target = null;
        }

        protected virtual bool NearPoint(Vector3 a, Vector3 b)
        {
            var _a = new Vector3(a.x, transform.position.y, a.z);
            var _b = new Vector3(b.x, transform.position.y, b.z);
            return Vector3.Distance(_a, _b) <= 0.5f;
        }
    }

}