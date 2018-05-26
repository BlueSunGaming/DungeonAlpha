
using System;
using System.Linq;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    [CustomEditor(typeof(WorldLootItemMono))]
    public class WorldLootItemMonoEditor: UnityEditor.Editor
    {
        SerializedProperty worldLootItem;
        SerializedProperty model; 
        SerializedProperty canvas;
        SerializedProperty itemText;

        SerializedProperty bookCanvas;
        SerializedProperty useBookText;

        private int selectedType = 0;
        private int selectedItem = 0;
        private string[] types = new string[]{"Item","Craftable Item","Quest Item"};

        void OnEnable()
        {
            worldLootItem = serializedObject.FindProperty("LootItem");
            canvas = serializedObject.FindProperty("Canvas");
            bookCanvas = serializedObject.FindProperty("BookCanvas");
            model = serializedObject.FindProperty("Model");
            itemText = serializedObject.FindProperty("ItemText");
            useBookText = serializedObject.FindProperty("UseBookText");
        }   

        public override void OnInspectorGUI()
        {
            if (Rm_RPGHandler.Instance == null) return;

            EditorGUILayout.LabelField("Select Item:");


            serializedObject.Update();
            var w = worldLootItem.FindPropertyRelative("Type");
            var s = worldLootItem.FindPropertyRelative("ItemId");
            var q = worldLootItem.FindPropertyRelative("Quantity");
            var lootOnce = worldLootItem.FindPropertyRelative("LootInWorldOnce");
            var lootId = worldLootItem.FindPropertyRelative("LootId");
            selectedType = w.intValue;
            var oldSelectedType = selectedType;
            selectedType = EditorGUILayout.Popup("- Item Type:", selectedType, types);
            if(oldSelectedType != selectedType)
            {
                selectedItem = 0;
            }
            w.intValue = selectedType;

            string[] foundItems;
            switch(selectedType)
            {
                case 0:
                    foundItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems.Select(i => i.Name).ToArray();
                    break;
                case 1:
                    foundItems = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.Select(i => i.Name).ToArray();
                    break;
                case 2:
                    foundItems = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems.Select(i => i.Name).ToArray();
                    break;
                default:
                    foundItems = new string[0];
                    break;
            }

            if(!string.IsNullOrEmpty(s.stringValue))
            {                
                Item foundItem;
                switch (selectedType)
                {
                    case 0:
                        foundItem = Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(i => i.ID == s.stringValue);
                        break;
                    case 1:
                        foundItem = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.FirstOrDefault(i => i.ID == s.stringValue);
                        break;
                    case 2:
                        foundItem = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems.FirstOrDefault(i => i.ID == s.stringValue);
                        break;
                    default:
                        foundItem = null;
                        break;
                }

                if(foundItem != null)
                    selectedItem = Array.IndexOf(foundItems, foundItem.Name);
            }

            if(foundItems.Length > 0)
            {
                selectedItem = EditorGUILayout.Popup("- Item:", selectedItem, foundItems);    
            }
            else
            {
                EditorGUILayout.LabelField("- Item:","None Found");
            }

            Item item;
            switch (selectedType)
            {
                case 0:
                    item = Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(i => i.Name == foundItems[selectedItem]);
                    s.stringValue = item != null ? item.ID : "";
                    break;
                case 1:
                    item = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.FirstOrDefault(i => i.Name == foundItems[selectedItem]);
                    s.stringValue = item != null ? item.ID : "";
                    break;
                case 2:
                    item = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems.FirstOrDefault(i => i.Name == foundItems[selectedItem]);
                    s.stringValue = item != null ? item.ID : "";
                    break;
                default:
                    item = null;
                    s.stringValue = "";
                    break;
            }

            if(item != null)
            {
                var stackable = item as IStackable;
                if(stackable != null)
                {
                    EditorGUILayout.PropertyField(q, new GUIContent("- Quantity:"));
                }
                else
                {
                    q.intValue = 1;
                }
            }
            EditorGUILayout.PropertyField(lootOnce, new GUIContent("- Loot In World Only Once:"));
            EditorGUILayout.PropertyField(lootId, new GUIContent("- Loot Id:"));

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Gameobject References:");
            EditorGUILayout.PropertyField(canvas, new GUIContent("- Canvas:"));
            EditorGUILayout.PropertyField(itemText, new GUIContent("- Item Text:"));
            EditorGUILayout.PropertyField(bookCanvas, new GUIContent("- Book Canvas:"));
            EditorGUILayout.PropertyField(useBookText, new GUIContent("- Use Book Text:"));

            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
