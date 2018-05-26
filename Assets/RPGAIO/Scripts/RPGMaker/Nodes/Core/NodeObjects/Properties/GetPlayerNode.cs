using System;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class GetPlayerNode : PropertyNode
    {
        public override string Name
        {
            get { return "Get Player GameObject"; }
        }

        public override string Description
        {
            get { return "Gets the player's gameObject."; }
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
            var property = GetObject.PlayerMonoGameObject;
            var result = func(property);
            return result;
        }
    }
}