using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Invector.vShooter
{
    using vCharacterController;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(vShooterManager), true)]
    public class vShooterManagerEditor : vEditorBase
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            vShooterManager manager = (vShooterManager)this.target;           
            base.OnInspectorGUI();
            var color = GUI.color;
            if (Application.isPlaying && manager.tpCamera)
            {
                GUI.color = Color.red;
                EditorGUILayout.BeginVertical(skin.box);
                GUI.color = color;
                EditorGUILayout.HelpBox("Playmode Debug - Equip a weapon first", MessageType.Info);
                EditorGUILayout.Space();

                if (GUILayout.Button(manager.tpCamera.lockCamera ? "Unlock Camera" : "Lock Camera", EditorStyles.toolbarButton))
                {
                    manager.tpCamera.lockCamera = !manager.tpCamera.lockCamera;
                }
                EditorGUILayout.Space();
                if (GUILayout.Button(manager.alwaysAiming ? "Unlock Aiming" : "Lock Aiming", EditorStyles.toolbarButton))
                {
                    manager.alwaysAiming = !manager.alwaysAiming;
                }
                EditorGUILayout.Space();
                if (GUILayout.Button(manager.showCheckAimGizmos ? "Hide Aim Gizmos" : "Show Aim Gizmos", EditorStyles.toolbarButton))
                {
                    manager.showCheckAimGizmos = !manager.showCheckAimGizmos;
                }
                EditorGUILayout.EndVertical();
            }
            GUI.color = color;         
        }
    }
}