Thank you for support our asset!

*IMPORTANT* This asset requires Unity 5.6.1 or higher.

If you have any question about how it works or if you are experiencing any trouble, 
feel free to email us at: inv3ctor@gmail.com
Please do not Upload or share this asset as a package without permission.

If you downloaded this asset illegally for studies or prototype purposes, 
please reconsider purchase if you want to publish your work, you can buy on the AssetStore, the vStore
or send us a email and we can figure something out, you can even post your work on our Forum, 
will be happy to help and advertise your game.

It has been more than 1 year since the release of v1.0 and we continue to work on this only because of your support, 
otherwise we will have to find day jobs and we would never had time to work on this, so thank you!

ASSETSTORE: https://www.assetstore.unity3d.com/en/#!/content/44227
VSTORE: https://sellfy.com/invector
FORUM: http://invector.proboards.com/
YOUTUBE: https://www.youtube.com/channel/UCSEoY03WFn7D0m1uMi6DxZQ
WEBSITE: http://www.invector.xyz/
PATREON: https://www.patreon.com/invector

Invector Team - 2018

Shooter v1.2.0 CORE UPDATE- 16/03/2018

- add MeleeClickToMove demo scene (Diablo combat style)
- add Jump Multiplier Spring example in the Basic Locomotion scene
- add Archery System for No-Inventory scene (Shooter only)
- add namespace on all vScripts
- add vHealthController (You can use this component to have health into generic objects without the need of a vCharacter which now inherits from the vHealthController)
- add IK offset for left hand
- add OnEnterLadder/Exit Events
- convert Legacy Particles to Shuriken 
- fix rotation bug with Generic Actions and Ladder
- fix Basic Locomotion tab not showing in Mac OS devices
- fix ScopeView rotation when crouched
- fix Ragdoll issues 
- improved Standalone Character 
- improve attack exit transitions smoothness
- update several scripts to avoid over warnings using Unity last API 
- update and fixed several prefabs and scenes
- update project to Unity 5.6.1 

-----------------------------------------------------------------------------------------------------

Shooter v1.1.5 ARCHERY UPDATE - 08/01/2018

* Happy New Year!!

- add archery support with collectable arrows
- add secondary shot with vRifle example 
- add support for charge shot and multiple projectiles (see vRifle example)
- add leaning animations for walk, run and sprint animations
- add new unarmed moveset for free, strafe and crouch 
- add new pick up item animations
- add SetLockShooterInput, SetLockMeleeInput and SetLockBasicInput to call on Events and lock individual inputs
- add ShowCursor, LockCursor and SetLockCameraInput methods to call on Events
- add new CheckGroundMethod with options to Low and High detections levels
- updated AmmoManager to automatically creates a ammoType
- updated vPlatform to work with the FreeClimb Add-on
- updated all animator controllers (*important - make sure to update your old animator based on the new)
- fix camera bugs
- fix animator looping some animations on 2017.3

-----------------------------------------------------------------------------------------------------

Shooter v1.1.4 HOTFIX - 31/10/2017

- changes in the tpInput to update the Adventure Creator & Playmaker Integration
- slopeLimit improved and add slide velocity in the inspector
- fix bugs in the 2.5D scene, player animator needs to be on update mode animatePhysics
- fix lock-on target not exiting lock-on mode with more then 1 target close
- add aimMinDistance for the TopDownShooter

-----------------------------------------------------------------------------------------------------

Shooter v1.1.3 HIPFIRE SHOOTER / HOTFIX - 05/10/2017

- add hipfire shot (ability to shot without aim) 
- add hipfire dispersion (how much the shot will disperse while shooting without aiming)
- add camera sway (random camera movement while aiming) 
- shooter weapon add precision (how much precision the weapon have, 1 means no cameraSway and 0 means maxCameraSway)
- fix onDead event not being called on vCharacterStandalone
- fix ragdoll RemovePhysicsAfterDie option
- fix ragdoll causing error when dying on spikes
- fix weapon holder bug (when pickup a new weapon, the current weapon holder show/hide)

-----------------------------------------------------------------------------------------------------

Shooter v1.1.2 TOPDOWN SHOOTER / HOTFIX - 18/09/2017

- add support for topdown shooter aim height
- add support to quickly change the CameraState using the ChangeCameraState method from tpInput
- add support to create Ragdolls for Generic Rigs 
- add Ragdoll Generic Template if you have several models with the same hierarchy name, add the bone name once and create for every model
- fix weapon handler equip delay time
- fix roll direction bug when using strafe
- fix isAiming not reseting when the weapon is destroyed
- fix isReloading not reseting when the weapon is destroyed
- fix strafeLimit not working when walkByDefault is enable
- change animator update mode back to normal 
- change ColorSpace back to Gama (default color space of Unity)
- minor improvements 

-----------------------------------------------------------------------------------------------------

Shooter v1.1.1 HOTFIX- 30/08/2017

- fix strafe movement speed 
- fix melee manager not causing any damage on generic custom hitboxes
- fix input timer for shooter weapons, it's more precise now (you may need to change the frequency value)
- fix camera jittering when using the MeleeLockOn
- fix OnCheckInvalidAim not displaying the "X" when can't aim
- fix headTrack sync issue with shooterAimIK
- fix katana 3D Model importing errors on 5.5.0
- fix speed of holding melee weapons walk animations
- fix mobile inventory not equiping weapons
- improve shoot coroutines 
- change shootFrequency values of the example weapons
- add free movement with lockOn (use the bool strafeWhileLockOn) 
- add support to shoot bodyParts on generic rigs without using a ragdoll (using vCollisionMessage)
- add API link under the tab Help

-----------------------------------------------------------------------------------------------------

Shooter v1.1.0 TOPDOWN/2.5D/THROW- 17/08/2017

- add 2.5D Shooter Demo scene
- add 2.5D Curved path system
- add camera trigger to change angle
- add Topdown movement with rotation based on mousePosition 
- add Throw system (works like add-on, plug & play)
- add TopDown & Throw System Demo Scene
- add LockMeleeInput & LockShooterInput option to use with events (see topdown example)
- remove ClickToMove from the Core, now different types of controller have their own scripts making the core more clean
- change the GenericAnimation to have a external way to call the method PlayAnimation without input
- improved aim IK behaviour smoothness
- improved camera behaviour smoothness
- improved charater rotation smoothness
- fix Camera StartUsingTransformRotation and StartSmooth options
- fix footstep particle instantiating without layer verification
- fix stamina consumption while crouching or jumping
- fix bullet life settings bugs

-----------------------------------------------------------------------------------------------------

Shooter v1.1d HOTFIX UPDATE - 19/07/2017

- add bullet life settings and options to pass through objects (see vShooterOnly_WITHInventory demo scene)
- add katana 2 hand moveset & attack animations 
- add revival option for both Player & AI
- add new inspector icons for important scripts
- creating a melee controller now adds the meleeManager and lock-on automatically
- footstep has the defaultSurface assign when add the component
- change move speed variables to a internal class 
- fix reaction/recoil/death animations playing without root motion
- fix ragdoll bug disappearing the character after the 2 hit
- fix mouse click not working in the inventory on editor
- fix/improved turnOnSpot verifications

-----------------------------------------------------------------------------------------------------

Shooter v1.1c IMPROVEMENTS UPDATE - 30/6/2017

- add inventory support for Mobile
- add Inventory DemoScene with examples on how to add/remove/equip/unequip/destroy/drop/check items
- add vItemCollectionDisplay prefab (hud text showing what you collect from the vItemCollection)
- add keycard examples to open doors
- add support to revive the Player 
- add option to remove components when the player dies
- add option to start the camera without the player rotation and without lerp
- add input to go up/down for the ladder
- fixed StepOffset Raycast interfering with Triggers making the character float a little bit 
- fixed headtrack look at while aiming bug
- new small features for the scene 2.5D, lock Z axis and remove vertical input
- the Lock-On component is now a add-on and should be attached into the Player instead of the Camera
- improved FootStep logic
- remove all SendMessage calls from the project
- several small fixes and improvements requested by add-on creators to improve compatibility
- controller is now setup to be without root motion by default, to improve the range of adding new custom animations

-----------------------------------------------------------------------------------------------------

Shooter v1.1b HOTFIX/INVENTORY - 5/6/2017

- updated documentation
- improved ladder action component
- improved jump animation transition
- add strong unarmed attack animations
- add option to auto equip melee weapons, and set the equiArea (melee, consumable, etc) 
- add SlotIdentifier on the EquipDisplay so it's easier to know what slot you're in
- fix chests in the melee demo scene
- fix occasionally 360 camera spin after enemy die with lock-on activated
- fix weapon holders bugs 

-----------------------------------------------------------------------------------------------------

Shooter v1.1a HOTFIX - 31/5/2017

- Fix missing BodyMembers in the MeleeManager
- Fix camera reposition to 0, 0, 0 in the Editor
- Add Auto Equip option for the Item & ItemCollection
- Add vRemoveCurrentItem as example to Unequip, Drop or Destroy the current item equipped
- Add vSimpleTrigger script with simple trigger verifications with Events
- Small changes in the vGenericAction 

-----------------------------------------------------------------------------------------------------

Shooter v1.1 - 22/5/2017

- New Action system implemented
- New in-game HUD add 
- New TriggerAction with more options for MatchTarget
- New LadderAction separated from the controller
- New vSkin for the Editors
- Add Events for the Controller
- Add RandomAttacks example
- Add support for Shooter & Melee together without inventory 
- Add support to drag and drop a shooter weapon on start
- Fix Twisted Model on the mobile demo scene
- Fix missing particles prefabs 
- Fix Jump stuck animation in the ShooterMelee Animator
- Removed ActionText from the HUD
- Several minor changes to improve stability

-----------------------------------------------------------------------------------------------------

Changelog v1.0a HOTFIX xx/04/2017

Fix: 
- removed exit time transition of jump on the shooterMelee animator

Changes:
- attack stamina is now deal at the attack state as requested by users
- small changes to improve compatibility with custom add-ons & integrations

-----------------------------------------------------------------------------------------------------

Shooter v1.0 - xx/03/2017
- First Release based on MeleeCombat Template v2.1