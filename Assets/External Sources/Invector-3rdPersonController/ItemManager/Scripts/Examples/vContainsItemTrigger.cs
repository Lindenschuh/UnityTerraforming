using UnityEngine;
using System.Collections;

namespace Invector.vItemManager
{
    [vClassHeader("Contains Item Trigger", "Simple trigger to check if the Player has a specific Item, you also can use Events to trigger something in case you have the item.")]
    public class vContainsItemTrigger : vMonoBehaviour
    {
        public bool getItemByName;
        [vHideInInspector("getItemByName")]
        public string itemName;
        [vHideInInspector("getItemByName", true)]
        public int itemID;
        public bool useTriggerStay;
        [Header("OnTriggerEnter/Stay")]
        public UnityEngine.Events.UnityEvent onContains;
        public UnityEngine.Events.UnityEvent onNotContains;
        [Header("OnTriggerExit")]
        public UnityEngine.Events.UnityEvent onExit;

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var itemManager = other.GetComponent<vItemManager>();
                if (itemManager)
                {
                    CheckItem(itemManager);
                }
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (!useTriggerStay) return;
            if (other.gameObject.CompareTag("Player"))
            {
                var itemManager = other.GetComponent<vItemManager>();
                if (itemManager)
                {
                    CheckItem(itemManager);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onExit.Invoke();
            }
        }

        protected virtual void CheckItem(vItemManager itemManager)
        {
            if (getItemByName)
            {
                if (itemManager.ContainItem(itemName)) onContains.Invoke();
                else onNotContains.Invoke();
            }
            else
            {
                if (itemManager.ContainItem(itemID)) onContains.Invoke();
                else onNotContains.Invoke();
            }
        }
    }

}
