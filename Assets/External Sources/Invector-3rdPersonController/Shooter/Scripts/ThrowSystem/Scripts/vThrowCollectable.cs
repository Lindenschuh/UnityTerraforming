using UnityEngine;
using System.Collections;
namespace Invector.vCharacterController
{
    [vClassHeader("THROW COLLECTABLE", false)]
    public class vThrowCollectable : vMonoBehaviour
    {
        public int amount = 1;
        public bool destroyAfter = true;
        vThrowObject throwManager;

        public UnityEngine.Events.UnityEvent onCollectObject;
        public UnityEngine.Events.UnityEvent onReachMaxObjects;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
                throwManager = other.GetComponent<vThrowObject>();
        }

        public void UpdateThrowObj(Rigidbody throwObj)
        {
            if (throwManager.currentThrowObject < throwManager.maxThrowObjects)
            {
                throwManager.SetAmount(amount);
                throwManager.objectToThrow = throwObj;
                onCollectObject.Invoke();
                if (destroyAfter) Destroy(this.gameObject);
            }
            else
            {
                onReachMaxObjects.Invoke();
            }
        }
    }
}