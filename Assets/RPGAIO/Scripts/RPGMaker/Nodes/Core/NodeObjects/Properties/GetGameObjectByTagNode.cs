using System;
using Assets.Scripts.Exceptions;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class GetGameObjectByTagNode : PropertyNode
    {
        public override string Name
        {
            get { return "Get GameObject By Tag"; }
        }

        public override string Description
        {
            get { return "Gets a gameObject by tag."; }
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
            Add("Tag", PropertyType.String, null, null, PropertySource.EnteredOrInput);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var gameObjectTag = (string) ValueOf("Tag");
            var property = GameObject.FindGameObjectWithTag(gameObjectTag);
            if(property == null) throw new NodeParameterNotFoundException("GameObject is null.");
            var result = func(property);
            return result;
        }
    }
}