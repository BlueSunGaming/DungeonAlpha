using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor.New
{
    public class EventNodeWindow : NodeWindow
    {
        public static EventNodeWindow Window;

        [MenuItem("Tools/LogicSpawn RPG All In One/Node Editors/Events", false, 1)]
        public static void Init()
        {
            Window = GetWindow<EventNodeWindow>("Events",typeof(CombatNodeWindow),typeof(DialogNodeWindow),typeof(WorldMapNodeWindow), typeof(AchievementNodeWindow), typeof(EventNodeWindow));
            Window.minSize = new Vector2(1100.0F, 450.0F);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Window = this;
        }

        protected internal override NodeBank NodeBank
        {
            get { return Rm_RPGHandler.Instance.Nodes.EventNodeBank; }
        }

        protected override NodeTreeType GetNodeType()
        {
            return NodeTreeType.Event;
        }

        protected override void FilterNodeTypes(ref List<Type> nodeTypes)
        {

        }
    }
}