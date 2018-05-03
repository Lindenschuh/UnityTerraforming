using UnityEngine;
using System.Collections;
namespace Invector.vItemManager
{
    public class vAddItemByID : MonoBehaviour
    {
        public int id, amount;
        public bool autoEquip;
        public bool destroyAfter;

        /// <summary>
        /// Simple example on how to add one or more items into the inventory using code
        /// You can also auto equip the item if it's a MeleeWeapon Type
        /// </summary>
        /// <param name="other"></param>
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var itemManager = other.gameObject.GetComponent<vItemManager>();
                if (itemManager)
                {
                    var reference = new ItemReference(id);
                    reference.amount = amount;
                    reference.autoEquip = autoEquip;
                    itemManager.AddItem(reference);
                }
                if (destroyAfter) Destroy(gameObject);
            }
        }
    }
}

