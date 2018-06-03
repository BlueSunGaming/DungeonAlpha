using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEditor;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Items_CostumeDesigner
    {
        private static Rect leftAreaB;
        private static Rect mainAreaAlt;
        private static GameObject selectedClass;
        private static Transform selectedChildTransform;
        private static Vector2 hierarchyScrollView = Vector2.zero;
        private static GameObject[] classPrefabs;
        private static bool showItems = true;
        private static string _searchTerm = "";
        private static string _searchTermPlaceholder = "<color=gray>Type here to filter: </color>";
        public static void Main(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            leftAreaB = new Rect(leftArea.xMax + 5, leftArea.y, leftArea.width, leftArea.height);
            mainAreaAlt = new Rect(leftAreaB.xMax + 5, leftArea.y, mainArea.width - (leftAreaB.width + 5),
                                    leftArea.height);

            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(leftAreaB, "", "backgroundBox");
            GUI.Box(mainAreaAlt, "", "backgroundBox");



            GUILayout.BeginArea(PadRect(leftArea, 1, 1));

            if(classPrefabs == null)
            {
                classPrefabs = Resources.LoadAll<GameObject>("Prefabs/Classes");    
            }

            try
            {
                //Get class prefabs in class prefab location
                foreach (var classPrefab in classPrefabs)
                {
                    var ifSelected = selectedClass == classPrefab ? "listItemSelected" : "listItem";
                    if (GUILayout.Button(classPrefab.name, ifSelected))
                    {
                        selectedClass = classPrefab;
                        Debug.Log("clicked class  " + selectedClass.name);
                    }
                }    
            }
            catch(Exception e)
            {
                Debug.Log("Failed loading class for costume designer, reloading class prefabs");
                classPrefabs = Resources.LoadAll<GameObject>("Prefabs/Classes");
                GUILayout.EndArea();
                return;
            }
            

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("- Reload Classes -", "listItem"))
            {
                classPrefabs = Resources.LoadAll<GameObject>("Prefabs/Classes"); 
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(PadRect(leftAreaB, 1, 2));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Filter:",GUILayout.Width(35));
            GUI.SetNextControlName("searchTerm");
            _searchTerm = GUILayout.TextField(_searchTerm, "nodeTextField");
            GUILayout.Space(1);
            GUILayout.EndHorizontal();
            if (Event.current.type == EventType.Repaint)
            {
                if (GUI.GetNameOfFocusedControl() == "searchTerm" && _searchTerm == _searchTermPlaceholder)
                {
                    _searchTerm = "";
                }
            }

            hierarchyScrollView = GUILayout.BeginScrollView(hierarchyScrollView);
            if(selectedClass != null)
            {
                var children = selectedClass.GetAllChildren<Transform>();
                if (_searchTerm != _searchTermPlaceholder)
                {
                    children = children.Where(n => n.name.ToLower().Contains(_searchTerm.ToLower())).ToArray();
                }
                foreach(var child in children)
                {
                    var ifSelected = selectedChildTransform == child ? "listItemSelected" : "listItem";
                    if (GUILayout.Button(child.name, ifSelected))
                    {
                        selectedChildTransform = child;
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Pick a class to choose a child transform to enable/disable depending on an equipped item.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(mainAreaAlt);
            if(selectedChildTransform != null)
            {
                var classDef =Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(c => c.Name == selectedClass.name);
                if(classDef != null)
                {
                    var result = RPGMakerGUI.FoldoutToolBar(ref showItems, "Items required to show", "+Item");
                    if (showItems)
                    {
                        var defs = classDef.EquipmentInfo.Definitions.Where(d => d.NameOfTransform == selectedChildTransform.name).ToList();

                        for (int i = 0; i < defs.Count; i++)
                        {
                            var dynamicDef = defs[i];
                            GUILayout.BeginHorizontal();
                            //RPGMakerGUI.PopupID<Item>("Item:", ref dynamicDef.RequiredEquippedItemId);
                            RPGMakerGUI.PopupID<Item>("Item:", ref dynamicDef.RequiredEquippedItemId, "ID", "Name", "CraftItemAndQuest");

                            if (RPGMakerGUI.DeleteButton(25))
                            {
                                classDef.EquipmentInfo.Definitions.Remove(dynamicDef);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                            RPGMakerGUI.Toggle("Only In Weapon Slot?", ref dynamicDef.OnlyWeaponSlot);
                            if (!dynamicDef.OnlyWeaponSlot && RPGMakerGUI.Toggle("Only In Specific Slot?", ref dynamicDef.SpecificSlot))
                            {
                                RPGMakerGUI.PopupID<SlotDefinition>("Slot:", ref dynamicDef.SlotId);
                            }

                            GUILayout.Space(20);
                        }
                        foreach(var dynamicDef in defs)
                        {
                            
                        }

                        if (result == 0)
                        {
                            classDef.EquipmentInfo.Definitions.Add(new DynamicEquipmentDefinition(selectedChildTransform.name));
                        }

                        if(!classDef.EquipmentInfo.Definitions.Any())
                        {
                            EditorGUILayout.HelpBox("Click +Item to add an item which will activate this transform", MessageType.Info);
                        }

                        RPGMakerGUI.EndFoldout();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("This character does not exist in your character definitions. Rename the prefab in 'Resources/Prefabs/Classes' to be the same as your character in RPGAIO > Player > Characters.", MessageType.Error);
                }
                
            }
            else
            {
                EditorGUILayout.HelpBox("Pick a child transform to choose an item for it to depend on.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        
        private static Rect RoundWindow(Rect rect)
        {
            rect.x = rect.x.RoundToNearest(5f);
            rect.y = rect.y.RoundToNearest(5f);
            return rect;
        }

        private static Rect ClampWindow(Rect rect, Rect toRect)
        {
            if(Event.current.type == EventType.Repaint)
            {
                rect.x = Mathf.Clamp(rect.x, toRect.xMin, toRect.xMax - rect.width);
                rect.y = Mathf.Clamp(rect.y, toRect.yMin, toRect.yMax - rect.height);

                return rect;
            }
            return rect;
            
        }

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }
}