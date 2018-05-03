using UnityEngine;
using System.Collections;
namespace Invector.vCharacterController
{
    public class vMousePositionHandler : MonoBehaviour
    {
        protected static vMousePositionHandler _instance;
        public static vMousePositionHandler Instance
        {
           get
            {
                if (_instance == null) _instance = FindObjectOfType<vMousePositionHandler>();
                if (_instance == null)
                {
                    var go = new GameObject("MousePositionHandler");
                    _instance = go.AddComponent<vMousePositionHandler>();
                }
                return _instance;
            }
        }

        public string joystickHorizontalAxis = "RightAnalogHorizontal";
        public string joystickVerticalAxis= "RightAnalogVertical";
        public float joystickSensitivity = 25f;       
        Vector2 joystickMousePos;
        public virtual Vector2 mousePosition
        {
            get
            {
                var inputDevice = vInput.instance.inputDevice;
                switch (inputDevice)
                {
                    case InputDevice.MouseKeyboard:
                        return Input.mousePosition;
                    case InputDevice.Joystick:
                        joystickMousePos.x += Input.GetAxis("RightAnalogHorizontal") * joystickSensitivity;
                        joystickMousePos.x = Mathf.Clamp(joystickMousePos.x, -(Screen.width * 0.5f), (Screen.width * 0.5f));
                        joystickMousePos.y += Input.GetAxis("RightAnalogVertical") * joystickSensitivity;
                        joystickMousePos.y = Mathf.Clamp(joystickMousePos.y, -(Screen.height * 0.5f), (Screen.height * 0.5f));
                        var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                        var result = joystickMousePos + screenCenter;
                        result.x = Mathf.Clamp(result.x, 0, Screen.width);
                        result.y = Mathf.Clamp(result.y, 0, Screen.height);
                        return result;
                    case InputDevice.Mobile:
                        return Input.GetTouch(0).deltaPosition;

                    default: return Input.mousePosition;
                }
            }
        }

        public virtual Vector3 WorldMousePosition(LayerMask castLayer)
        {
            if (!Camera.main)
            {
               Debug.LogWarning("Trying get the world mouse position but does not have a MainCamera in this Scene");
               return Vector3.zero;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, castLayer)) return hit.point;
                else return ray.GetPoint(Camera.main.farClipPlane);
            }
        }
    }
}

