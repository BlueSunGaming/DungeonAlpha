using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor.New
{
    public class AchievementNodeWindow : NodeWindow
    {
        public static AchievementNodeWindow Window;

        [MenuItem("Tools/LogicSpawn RPG All In One/Node Editors/Achievements", false, 1)]
        public static void Init()
        {
            Window = GetWindow<AchievementNodeWindow>("Achievements", typeof(CombatNodeWindow), typeof(DialogNodeWindow), typeof(WorldMapNodeWindow), typeof(AchievementNodeWindow), typeof(EventNodeWindow));
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
            FocusWindowIfItsOpen(typeof(AchievementNodeWindow));
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
            get { return Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank; }
        }

        protected override NodeTreeType GetNodeType()
        {
            return NodeTreeType.Achievements;
        }

        protected override void FilterNodeTypes(ref List<Type> nodeTypes)
        {
            nodeTypes = nodeTypes.Where(n => !GetNodeObject(n).IsRoutine).ToList();
        }
    }
}