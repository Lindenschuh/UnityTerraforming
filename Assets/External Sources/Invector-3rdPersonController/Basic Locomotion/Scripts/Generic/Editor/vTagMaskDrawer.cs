using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
[CustomPropertyDrawer(typeof(vTagMask),true)]
public class vTagMaskDrawer : PropertyDrawer
{    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var values = CreateTags();
        ConvertTagsToValue(property, values);

        EditorGUI.BeginProperty(position, label, property);
        property.FindPropertyRelative("value").intValue = EditorGUI.MaskField(position,label, property.FindPropertyRelative("value").intValue, values,EditorStyles.popup);
        EditorGUI.EndProperty();
        if (GUI.changed)
            CheckValues(values, property.FindPropertyRelative("value").intValue, property);
      
    }

    public void ConvertTagsToValue(SerializedProperty property,string[] maGroupNames)
    {
        var tags = property.FindPropertyRelative("tags");
        var value = property.FindPropertyRelative("value");
        value.intValue = 0;
        List<string> tagsSelected = new List<string>();
        for (int i = 0; i < tags.arraySize; i++)
        {
            tagsSelected.Add(tags.GetArrayElementAtIndex(i).stringValue);
        }
        for (int i = 0; i < maGroupNames.Length; i++)
        {
            if(tagsSelected.Contains(maGroupNames[i]))
            {
                int layer = 1 << i;
                value.intValue += layer;
            }
        }
    }
    public void CheckValues(string[] maGroupNames,int mCategory, SerializedProperty property)
    {
        List<string> valuesSelected = new List<string>();
        for (int i = 0; i < maGroupNames.Length; i++)
        {
            int layer = 1 << i;
            if ((mCategory & layer) != 0)
            {
                if (!valuesSelected.Contains(maGroupNames[i]))
                {
                    valuesSelected.Add(maGroupNames[i]);
                }
            }else  if(valuesSelected.Contains(maGroupNames[i]))
            {
                valuesSelected.Remove(maGroupNames[i]);
            }
        }
        property.FindPropertyRelative("tags").arraySize = valuesSelected.Count;
        for(int i =0; i< valuesSelected.Count; i++)
        {
            property.FindPropertyRelative("tags").GetArrayElementAtIndex(i).stringValue = valuesSelected[i];
        }
        property.serializedObject.ApplyModifiedProperties();
    }

    public string[]  CreateTags()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");
        if (tags == null || !tags.isArray)
        {
            Debug.LogWarning("Can't set up the tags.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Tags is null: " + (tags == null));
            return new string[0];
        }
        string[] list =new string[tags.arraySize+7];
        list[0] = "Untagged";
        list[1] = "Respawn";
        list[2] = "Finish";
        list[3] = "EditorOnly";
        list[4] = "MainCamera";
        list[5] = "Player";
        list[6] = "GameController";
        for (int a = 0; a < tags.arraySize; a++)
        {            
            SerializedProperty _tag = tags.GetArrayElementAtIndex(a);
            list[a+7] =(_tag.stringValue);
        }
        return list;
    }   
}
