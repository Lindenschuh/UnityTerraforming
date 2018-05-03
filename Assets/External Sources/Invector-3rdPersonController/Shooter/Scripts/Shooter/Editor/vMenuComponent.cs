using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Invector.vShooter
{
    public partial class vMenuComponent
    {
        [MenuItem("Invector/Shooter/Components/LockOn (Player Shooter Only)")]
        static void LockOnShooterMenu()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Invector.vCharacterController.vThirdPersonInput>() != null)
                Selection.activeGameObject.AddComponent<vLockOnShooter>();
            else
                Debug.Log("Please select a Player to add the component.");
        }
    }
}
