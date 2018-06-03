using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Interactables
    {
        public static Rmh_Interactables Interact
        {
            get { return Rm_RPGHandler.Instance.Interactables; }
        }
         public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             GUI.Box(fullArea, "", "backgroundBox");

             GUILayout.BeginArea(fullArea);
             RPGMakerGUI.Title("Interactables - Options");
             Interact.InteractDistance = RPGMakerGUI.FloatField("Interact Distance: ", Interact.InteractDistance);
             RPGMakerGUI.Toggle("Add Harvested Items to Inventory?", ref Interact.AddHarvestItemsToInventory);
             GUILayout.EndArea();
         }


         private static Interactable selectedInteractable = null;
         private static GameObject gameObject = null;
         private static bool showHarvestAnims = true;
         public static void InteractableObjects(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             var list = Rm_RPGHandler.Instance.Repositories.Interactable.AllInteractables;
             GUI.Box(leftArea, "","backgroundBox");
             GUI.Box(mainArea, "","backgroundBoxMain");

             GUILayout.BeginArea(PadRect(leftArea, 0, 0));
                RPGMakerGUI.ListArea(list, ref selectedInteractable, Rm_ListAreaType.Interactables, false, true);
             GUILayout.EndArea();


             GUILayout.BeginArea(mainArea);
             RPGMakerGUI.Title("Interactables");
             if (selectedInteractable != null)
             {
                 RPGMakerGUI.TextField("ID: ", selectedInteractable.ID);
                 selectedInteractable.Name = RPGMakerGUI.TextField("Name: ", selectedInteractable.Name);
                 GUILayout.BeginHorizontal();
                 gameObject = RPGMakerGUI.PrefabSelector("Prefab:", gameObject, ref selectedInteractable.PrefabPath);
                 gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Interactable, gameObject, ref selectedInteractable.PrefabPath, null, selectedInteractable.ID);
                 GUILayout.EndHorizontal();

                 selectedInteractable.Image = RPGMakerGUI.ImageSelector("Image", selectedInteractable.Image, ref selectedInteractable.ImagePath,true);
                 if (GUILayout.Button("Open Interaction Window", "genericButton", GUILayout.MaxHeight(30)))
                 {
                     var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                     var existingTree = trees.FirstOrDefault(t => t.ID == selectedInteractable.ID);
                     if (existingTree == null)
                     {
                         existingTree = NodeWindow.GetNewTree(NodeTreeType.Dialog);
                         existingTree.ID = selectedInteractable.ID;
                         existingTree.Name = selectedInteractable.Name;
                         trees.Add(existingTree);
                     }

                     DialogNodeWindow.ShowWindow(selectedInteractable.ID);
                     selectedInteractable.ConversationNodeId = existingTree.ID;
                 }

             }
             else
             {
                 EditorGUILayout.HelpBox("Add or select a new field to customise interactables.", MessageType.Info);
             }
             GUILayout.EndArea();
         }


         private static Harvestable selectedHarvestable = null;
        private static Vector2 areaScrollPos = Vector2.zero;
         public static void HarvestableObjects(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             var list = Rm_RPGHandler.Instance.Harvesting.HarvestableDefinitions;
             GUI.Box(leftArea, "","backgroundBox");
             GUI.Box(mainArea, "","backgroundBoxMain");

             GUILayout.BeginArea(PadRect(leftArea, 0, 0));
                RPGMakerGUI.ListArea(list, ref selectedHarvestable, Rm_ListAreaType.Harvestables, false, true);
             GUILayout.EndArea();


             GUILayout.BeginArea(mainArea);
             areaScrollPos = GUILayout.BeginScrollView(areaScrollPos);

             RPGMakerGUI.Title("Harvestable Objects");
             if (selectedHarvestable != null)
             {
                 RPGMakerGUI.TextField("ID: ", selectedHarvestable.ID);
                 selectedHarvestable.Name = RPGMakerGUI.TextField("Name: ", selectedHarvestable.Name);
                 GUILayout.BeginHorizontal();
                 gameObject = RPGMakerGUI.PrefabSelector("Prefab:", gameObject, ref selectedHarvestable.PrefabPath);
                 gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Harvest, gameObject, ref selectedHarvestable.PrefabPath, null, selectedHarvestable.ID);
                 GUILayout.EndHorizontal();

                 if (RPGMakerGUI.Toggle("Regenerates harvestables?", ref selectedHarvestable.RegensResources))
                 {
                     selectedHarvestable.TimeInSecToRegen = RPGMakerGUI.FloatField("Time before regen:", selectedHarvestable.TimeInSecToRegen);
                     selectedHarvestable.AmountRegenerated = RPGMakerGUI.IntField("Amount regenerated:", selectedHarvestable.AmountRegenerated);
                     selectedHarvestable.MaxAtOnce = RPGMakerGUI.IntField("Max At Once:", selectedHarvestable.MaxAtOnce);

                 }
                 else
                 {
                     selectedHarvestable.MinObtainable = RPGMakerGUI.IntField("Min Total Quantity Obtainable:", selectedHarvestable.MinObtainable);
                     selectedHarvestable.MaxObtainable = RPGMakerGUI.IntField("Max Total Quantity Obtainable:", selectedHarvestable.MaxObtainable);
                 }
                 
                 if (RPGMakerGUI.Toggle("Is Quest Item?", ref selectedHarvestable.IsQuestItem))
                 {
                     RPGMakerGUI.PopupID<Quest>("Accepted Quest Required To Loot:", ref selectedHarvestable.QuestAcceptedID);
                     RPGMakerGUI.PopupID<Item>("Harvested Quest Item:", ref selectedHarvestable.HarvestedObjectID, "ID", "Name", "Quest");
                     if (!string.IsNullOrEmpty(selectedHarvestable.HarvestedObjectID))
                     {
                         var item = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems.First(i => i.ID == selectedHarvestable.HarvestedObjectID);
                         var stackable = item as IStackable;
                         if (stackable != null)
                         {
                             selectedHarvestable.MinAmountGained = RPGMakerGUI.IntField("Min Quantity Gained:", selectedHarvestable.MinAmountGained);
                             selectedHarvestable.MaxAmountGained = RPGMakerGUI.IntField("Max Quantity Gained:", selectedHarvestable.MaxAmountGained);
                         }
                     }
                 }
                 else
                 {
                     RPGMakerGUI.PopupID<Item>("Harvested Item:", ref selectedHarvestable.HarvestedObjectID);
                     if (!string.IsNullOrEmpty(selectedHarvestable.HarvestedObjectID))
                     {
                         var item = Rm_RPGHandler.Instance.Repositories.Items.AllItems.First(i => i.ID == selectedHarvestable.HarvestedObjectID);
                         var stackable = item as IStackable;
                         if (stackable != null)
                         {
                             selectedHarvestable.MinAmountGained = RPGMakerGUI.IntField("Min Quantity Gained:", selectedHarvestable.MinAmountGained);
                             selectedHarvestable.MaxAmountGained = RPGMakerGUI.IntField("Max Quantity Gained:", selectedHarvestable.MaxAmountGained);
                         }
                     }
                 }

                 selectedHarvestable.HarvestSound = RPGMakerGUI.AudioClipSelector("Harvesting Sound:", selectedHarvestable.HarvestSound, ref selectedHarvestable.HarvestingSoundPath);
                 selectedHarvestable.TimeToHarvest = RPGMakerGUI.FloatField("Time to harvest:", selectedHarvestable.TimeToHarvest);
                 if(RPGMakerGUI.Toggle("Require Level To Harvest:", ref selectedHarvestable.RequireLevel))
                 {
                     selectedHarvestable.LevelRequired = RPGMakerGUI.IntField("- Required Level", selectedHarvestable.LevelRequired);
                 }

                 if(RPGMakerGUI.Toggle("Require Trait Level To Harvest:", ref selectedHarvestable.RequireTraitLevel))
                 {
                     RPGMakerGUI.PopupID<Rm_TraitDefintion>("- Trait", ref selectedHarvestable.RequiredTraitID);
                     selectedHarvestable.TraitLevelRequired = RPGMakerGUI.IntField("- Required Trait Level", selectedHarvestable.TraitLevelRequired);
                 }



                 Rme_Main_General.ProgressionGain(true, selectedHarvestable.ProgressionGain);

                 if(RPGMakerGUI.Foldout(ref showHarvestAnims, "Animations"))
                 {
                     foreach(var classHarvestAnim in selectedHarvestable.ClassHarvestingAnims)
                     {
                         var classInfo = RPG.Player.GetCharacterDefinition(classHarvestAnim.ClassID);
                         var className = classInfo.Name;
                         if(classInfo.AnimationType == RPGAnimationType.Legacy)
                         {
                             classHarvestAnim.LegacyAnim = RPGMakerGUI.TextField(className + " Animation:", classHarvestAnim.LegacyAnim);    
                         }
                         else
                         {
                             GUILayout.BeginHorizontal();
                             classHarvestAnim.AnimNumber = RPGMakerGUI.IntField(className + "Anim AnimNumber:", classHarvestAnim.AnimNumber);
                             GUILayout.EndHorizontal();
                         }
                     }

                     RPGMakerGUI.EndFoldout();
                 }

             }
             else
             {
                 EditorGUILayout.HelpBox("Add or select a new field to customise harvestable objects.", MessageType.Info);
             }
             GUILayout.EndScrollView();
             GUILayout.EndArea();
         }

         public static Rect PadRect(Rect rect, int left, int top)
         {
             return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
         }
    }
}