using UnityEngine;
using UnityEditor;

namespace Invector
{
    // MELEE COMBAT FEATURES
    public partial class vMenuComponent
    {
        [MenuItem("Invector/Melee Combat/Components/MeleeManager")]
        static void MeleeManagerMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<vMelee.vMeleeManager>();
            else
                Debug.Log("Please select a vCharacter to add the component.");
        }

        [MenuItem("Invector/Melee Combat/Components/WeaponHolderManager (Player Only)")]
        static void WeaponHolderMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Invector.vCharacterController.vThirdPersonInput>() != null)
                Selection.activeGameObject.AddComponent<Invector.vItemManager.vWeaponHolderManager>();
            else
                Debug.Log("Please select the Player to add the component.");
        }
        [MenuItem("Invector/Melee Combat/Components/LockOn (Player Only)")]
        static void LockOnMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Invector.vCharacterController.vThirdPersonInput>() != null)
                Selection.activeGameObject.AddComponent<vCharacterController.vLockOn>();
            else
                Debug.Log("Please select a Player to add the component.");
        }
    }
}
