using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Invector.vShooter
{
    [CustomEditor(typeof(vProjectileControl))]
    public class vProjectileControlEditor : Editor
    {
        public string[] ignoreProperties = new string[] { "onPassDamage", "onCastCollider" };
        public GUISkin skin, oldSkin;
        public bool openWindow;
        public Texture2D m_Logo;
        void OnEnable()
        {
            m_Logo = (Texture2D)Resources.Load("icon_v2", typeof(Texture2D));
        }
        public override void OnInspectorGUI()
        {

            oldSkin = GUI.skin;
            if (!skin) skin = Resources.Load("skin") as GUISkin;
            GUI.skin = skin;
            GUILayout.BeginVertical("Projectile Control", "window");
            GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
            GUILayout.Space(5);
            openWindow = GUILayout.Toggle(openWindow, openWindow ? "Close Controller Settings" : "Open Controller Settings", EditorStyles.toolbarButton);
            if (openWindow)
            {

                EditorGUILayout.HelpBox("The damage value is changed from minDamage, maxDamage, DropOffStart,DropOffEnd of the ShooterWeapon", MessageType.Info);
                DrawPropertiesExcluding(serializedObject, ignoreProperties);
                GUI.skin = oldSkin;
                for (int i = 0; i < ignoreProperties.Length; i++)
                {
                    var prop = serializedObject.FindProperty(ignoreProperties[i]);
                    if (prop != null) EditorGUILayout.PropertyField(prop);

                }
            }
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }

}
