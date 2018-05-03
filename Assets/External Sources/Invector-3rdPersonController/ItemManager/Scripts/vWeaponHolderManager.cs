using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Invector.vItemManager
{
    [vClassHeader("Weapon Holder Manager", "Create a new empty object inside a bone and add the vWeaponHolder script")]
    public class vWeaponHolderManager : vMonoBehaviour
    {
        public vWeaponHolder[] holders = new vWeaponHolder[0];
        [HideInInspector]
        public bool inEquip;
        [HideInInspector]
        public vItemManager itemManager;

        public Dictionary<string, List<vWeaponHolder>> holderAreas = new Dictionary<string, List<vWeaponHolder>>();
        protected float equipTime;

        void OnDrawGizmosSelected()
        {
            holders = GetComponentsInChildren<vWeaponHolder>(true);
        }

        void Start()
	    {
            itemManager = GetComponent<vItemManager>();
            if (itemManager)
            {
                itemManager.onEquipItem.AddListener(EquipWeapon);
                itemManager.onUnequipItem.AddListener(UnequipWeapon);
	            
                holders = GetComponentsInChildren<vWeaponHolder>(true);
                if (holders != null)
                {
                    foreach (vWeaponHolder holder in holders)
                    {
                        if (!holderAreas.ContainsKey(holder.equipPointName))
                        {
                            holderAreas.Add(holder.equipPointName, new List<vWeaponHolder>());
                            holderAreas[holder.equipPointName].Add(holder);
                        }
                        else
                        {
                            holderAreas[holder.equipPointName].Add(holder);
                        }

                        holder.SetActiveHolder(false);
                        holder.SetActiveWeapon(false);
                    }
                }
            }
        }

        public void EquipWeapon(vEquipArea equipArea, vItem item)
        {           
            var slotsInArea = equipArea.ValidSlots;

            if (slotsInArea != null && slotsInArea.Count > 0 && holderAreas.ContainsKey(equipArea.equipPointName))
            {
                //Check All Holders to Show
                for (int i = 0; i < slotsInArea.Count; i++)
                {
                    if (slotsInArea[i].item != null)
                    {
                        var holder = holderAreas[equipArea.equipPointName].Find(h => slotsInArea[i].item && slotsInArea[i].item.id == h.itemID
                        && ((equipArea.currentEquipedItem
                        &&  equipArea.currentEquipedItem != item 
                        &&  equipArea.currentEquipedItem != slotsInArea[i].item 
                        &&  equipArea.currentEquipedItem.id != item.id)|| !equipArea.currentEquipedItem));

                        if (holder)
                        {
                            holder.SetActiveHolder(true);
                            holder.SetActiveWeapon(true);                           
                        }
                    }
                }
                //Check Current Item to Equip with time
                if (equipArea.currentEquipedItem != null)
                {
                    var holder = holderAreas[equipArea.equipPointName].Find(h => h.itemID == equipArea.currentEquipedItem.id);
                    if (holder)
                    {
                        holder.equipDelayTime = equipArea.currentEquipedItem.equipDelayTime;
                        // Unhide Holder and hide Equiped weapon
                        StartCoroutine(EquipRoutine(holder,true, false, (itemManager.inventory != null && itemManager.inventory.isOpen)));
                    }
                }
            }     
        }

        public void UnequipWeapon(vEquipArea equipArea, vItem item)
        {
            if (holders.Length == 0 || item == null) return;
            
            if ((itemManager.inventory != null) && holderAreas.ContainsKey(equipArea.equipPointName))
            {
                var holder = holderAreas[equipArea.equipPointName].Find(h => item.id == h.itemID);
                if (holder)
                {                   
                    holder.equipDelayTime = item.equipDelayTime;
                    //Check if Equip area contains unequipped item
                    var containsItem = equipArea.ValidSlots.Find(slot => slot.item == item) != null; 
                    //Hide or unhide holder and weapon if contains item
                    StartCoroutine(UnequipRoutine(holder, containsItem, containsItem, (itemManager.inventory != null && itemManager.inventory.isOpen)));                   
                }
            }
        }        
      
        IEnumerator UnequipRoutine(vWeaponHolder holder, bool activeHolder, bool activeWeapon, bool immediat = false)
        {
            holder.SetActiveHolder(activeHolder);
            if (!inEquip && !immediat) // ignore time if inEquip or immediate unequip
            {
                equipTime = holder.equipDelayTime;
                while (equipTime > 0 && !immediat && !inEquip)
                {
                    yield return null;
                    equipTime -= Time.deltaTime;
                }
            } 
            holder.SetActiveWeapon(activeWeapon);
           
        }

        IEnumerator EquipRoutine(vWeaponHolder holder,bool activeHolder, bool activeWeapon, bool immediat = false)
        {
            if (!immediat)
	            inEquip = true;
            holder.SetActiveHolder(activeHolder);
            equipTime = holder.equipDelayTime;           
            while (equipTime > 0 && !immediat) // ignore time if  immediate equip
            {
                yield return null;
                equipTime -= Time.deltaTime;
            }           
          
            holder.SetActiveWeapon(activeWeapon);
            inEquip = false;
        }
    }
}
