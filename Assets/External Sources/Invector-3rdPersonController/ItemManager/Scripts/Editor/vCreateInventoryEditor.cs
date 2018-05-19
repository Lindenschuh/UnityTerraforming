﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using Invector;
using Invector.vCharacterController;
using System;

namespace Invector.vItemManager
{
    public class vCreateInventoryEditor : EditorWindow
    {
        GUISkin skin;
        vInventory inventoryPrefab;
        vItemListData itemListData;
        Vector2 rect = new Vector2(480, 210);
        Vector2 scrool;
        Texture2D m_Logo;

        [MenuItem("Invector/Inventory/ItemManager (Player Only)", false, 3)]
        public static void CreateNewInventory()
        {
            GetWindow<vCreateInventoryEditor>();
        }

        void OnGUI()
        {
            if (!skin) skin = Resources.Load("skin") as GUISkin;
            GUI.skin = skin;

            this.minSize = rect;
            this.titleContent = new GUIContent("Inventory System", null, "ItemManager Creator Window");
            m_Logo = Resources.Load("icon_v2") as Texture2D;
            GUILayout.BeginVertical("ItemManager Creator Window", "window");
            GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");            
            
            inventoryPrefab = EditorGUILayout.ObjectField("Inventory Prefab: ", inventoryPrefab, typeof(vInventory), false) as vInventory;
            itemListData = EditorGUILayout.ObjectField("Item List Data: ", itemListData, typeof(vItemListData), false) as vItemListData;

            if (inventoryPrefab != null && inventoryPrefab.GetComponent<vInventory>() == null)
            {
                EditorGUILayout.HelpBox("Please select a Inventory Prefab that contains the vInventory script", MessageType.Warning);
            }

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("Need to know how it works?");
            if (GUILayout.Button("Video Tutorial"))
            {
                //Application.OpenURL("https://www.youtube.com/watch?v=1aA_PU9-G-0&index=3&list=PLvgXGzhT_qehtuCYl2oyL-LrWoT7fhg9d");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (inventoryPrefab != null && itemListData != null)
            {
                if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<vThirdPersonController>() != null)
                {
                    if (GUILayout.Button("Create"))
                        Create();
                }
                else
                    EditorGUILayout.HelpBox("Please select the Player to add this component", MessageType.Warning);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Created the ItemManager
        /// </summary>
        void Create()
        {
            if (Selection.activeGameObject != null)
            {
                var itemManager = Selection.activeGameObject.AddComponent<vItemManager>();
                itemManager.inventoryPrefab = inventoryPrefab;
                itemManager.itemListData = itemListData;
                vItemManagerUtilities.CreateDefaultEquipPoints(itemManager,itemManager.GetComponent<vMelee.vMeleeManager>());                
            }
            else
                Debug.Log("Please select the Player to add this component.");

            this.Close();
        }
    }
}