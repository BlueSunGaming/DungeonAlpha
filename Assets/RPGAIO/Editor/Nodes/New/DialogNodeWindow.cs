using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor.New
{
    public class DialogNodeWindow : NodeWindow
    {
        public static DialogNodeWindow Window;
        [MenuItem("Tools/LogicSpawn RPG All In One/Node Editors/Dialog", false, 1)]
        public static void Init()
        {
            Window = GetWindow<DialogNodeWindow>("Dialog", typeof(CombatNodeWindow), typeof(DialogNodeWindow), typeof(WorldMapNodeWindow), typeof(AchievementNodeWindow), typeof(EventNodeWindow));
            Window.minSize = new Vector2(1100.0F, 450.0F);
        }

        public static void ShowWindow(string id)
        {
            if(Window == null)
            {
                Init();
            }

            Window.SelectedNodeTree = Window.NodeBank.NodeTrees.FirstOrDefault(n => n.ID == id);
            Window.SelectedNodeTreeIndex = Window.NodeBank.NodeTrees.IndexOf(Window.SelectedNodeTree);
            FocusWindowIfItsOpen(typeof(DialogNodeWindow));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Window = this;
        }

        protected internal override NodeBank NodeBank
        {
            get { return Rm_RPGHandler.Instance.Nodes.DialogNodeBank; }
        }

        protected override NodeTreeType GetNodeType()
        {
            return NodeTreeType.Dialog;
        }

        protected override void FilterNodeTypes(ref List<Type> nodeTypes)
        {
            //nodeTypes = nodeTypes.Where(n => !GetNodeObject(n).IsRoutine).ToList();
        }
    }
}