using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor.New
{
    public class WorldMapNodeWindow : NodeWindow
    {
        public static WorldMapNodeWindow Window;

        [MenuItem("Tools/LogicSpawn RPG All In One/Node Editors/World Map", false, 1)]
        public static void Init()
        {
            Window = GetWindow<WorldMapNodeWindow>("World Map", typeof(CombatNodeWindow), typeof(DialogNodeWindow), typeof(WorldMapNodeWindow), typeof(AchievementNodeWindow), typeof(EventNodeWindow));
            Window.minSize = new Vector2(1100.0F, 450.0F);
        }

        public static void ShowWindow(string id)
        {
            if (Window == null)
            {
                Init();
            }

            Window.SelectedNodeTree = Window.NodeBank.NodeTrees.FirstOrDefault(n => n.ID == id);
            Window.SelectedNodeTreeIndex = Window.NodeBank.NodeTrees.IndexOf(Window.SelectedNodeTree);
            FocusWindowIfItsOpen(typeof(WorldMapNodeWindow));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Window = this;
        }

        protected override void Update()
        {
            base.Update();

        }

        protected internal override NodeBank NodeBank
        {
            get { return Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank; }
        }

        protected override NodeTreeType GetNodeType()
        {
            return NodeTreeType.WorldMap;
        }

        protected override void FilterNodeTypes(ref List<Type> nodeTypes)
        {
            nodeTypes = new List<Type>();
        }
    }
}