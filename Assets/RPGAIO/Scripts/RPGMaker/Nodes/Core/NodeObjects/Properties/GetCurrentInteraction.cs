using System;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class GetCurrentInteractionNode : PropertyNode
    {
        public override string Name
        {
            get { return "Get Interaction GameObject"; }
        }

        public override string Description
        {
            get { return "Gets the gameObject of the npc/interactable/harvestable you are interacting with."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override PropertyFamily PropertyFamily
        {
            get { return PropertyFamily.Object; }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return PropertyType.GameObject; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override void SetupParameters()
        {

        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var property = DialogHandler.Instance.DialogNpc.gameObject;
            var result = func(property);
            return result;
        }
    }
}