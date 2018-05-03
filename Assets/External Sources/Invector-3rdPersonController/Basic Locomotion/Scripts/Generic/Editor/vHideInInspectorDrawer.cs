using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
namespace Invector
{
    [CustomPropertyDrawer(typeof(vHideInInspectorAttribute),true)]
    public class vHideInInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _Position, SerializedProperty _Property, GUIContent _Label)
        { 
            vHideInInspectorAttribute _attribute = attribute as vHideInInspectorAttribute;
            
            if (_attribute != null && _Property.serializedObject.targetObject)
            {               
                var propertyName = _Property.propertyPath.Replace(_Property.name, "");
                var booleamProperties = _attribute.refbooleanProperty.Split(';');
                var valid = true;
                for (int i = 0; i < booleamProperties.Length; i++)
                {
                    var booleanProperty = _Property.serializedObject.FindProperty(propertyName + booleamProperties[i]);                  
                    if (booleanProperty != null)
                    {
                        valid = (bool)_attribute.invertValue ? !booleanProperty.boolValue : booleanProperty.boolValue;
                        if (!valid)
                        {
                            break;
                        }
                    }
                    else
                    {
                        
                        EditorGUI.PropertyField(_Position, _Property, true);
                    }
                }
                if (valid)
                {
                   
                    
                    EditorGUI.PropertyField(_Position, _Property, true);
                }                
            }
            else
                EditorGUI.PropertyField(_Position, _Property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            vHideInInspectorAttribute _attribute = attribute as vHideInInspectorAttribute;
            if (_attribute != null)
            {
                var propertyName = property.propertyPath.Replace(property.name, "");
                var booleamProperties = _attribute.refbooleanProperty.Split(';');
                var valid = true;
                for (int i = 0; i < booleamProperties.Length; i++)
                {
                    var booleamProperty = property.serializedObject.FindProperty(propertyName + booleamProperties[i]);
                    if (booleamProperty != null)
                    {
                        valid = _attribute.invertValue ? !booleamProperty.boolValue : booleamProperty.boolValue;
                        if (!valid) break;
                    }
                }
                if (valid) return base.GetPropertyHeight(property, label);
                else return -1;
            }
            return base.GetPropertyHeight(property, label);
        }
    }
}