using UnityEngine;
using System.Collections;
namespace Invector.vCharacterController
{
    [vClassHeader("Character Standalone")]
    public class vCharacterStandalone : vCharacter
    {
        /// <summary>
        /// 
        /// vCharacter Example - You can assign this script into non-Invector Third Person Characters to still use the AI and apply damage
        /// 
        /// </summary>

        [HideInInspector] public v_SpriteHealth healthSlider;
        
        protected override void Start()
        {
            base.Start();           
            Init();
        }

        /// <summary>
        /// TAKE DAMAGE - you can override the take damage method from the vCharacter and add your own calls 
        /// </summary>
        /// <param name="damage"> damage to apply </param>
        public override void TakeDamage(vDamage damage)
        {
            // don't apply damage if the character is rolling, you can add more conditions here
            if (isDead)
                return;
            base.TakeDamage(damage);           
            // apply vibration on the gamepad                    
            vInput.instance.GamepadVibration(0.25f);          
        }
    }
}