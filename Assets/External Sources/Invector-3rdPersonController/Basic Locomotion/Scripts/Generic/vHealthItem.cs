using UnityEngine;
using System.Collections;
namespace Invector
{
    using vCharacterController;
    public class vHealthItem : MonoBehaviour
    {
        
        [Tooltip("How much health will be recovery")]
        public float value;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                // access the basic character information
                var iChar = other.GetComponent<vThirdPersonController>();
                if (iChar != null)
                {
                    
                    // heal only if the character's health isn't full
                    if (iChar.currentHealth < iChar.maxHealth)
                    {
                        // limit healing to the max health
                        iChar.ChangeHealth((int)value);// currentHealth = Mathf.Clamp(targetHealth, 0, iChar.maxHealth);
                        Destroy(gameObject);
                    }
                    else
                    {
                        var tpInput = other.GetComponent<vThirdPersonInput>();
                        // show message if the character's health is full
                        tpInput.hud.ShowText("Health is Full");
                    }
                }
            }
        }
    }
}