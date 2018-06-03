using System;
using Assets.Scripts.Exceptions;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject","")]
    public class GetGameObjectByNameNode : PropertyNode
    {
        public override string Name
        {
            get { return "Get GameObject By Name"; }
        }

        public override string Description
        {
            get { return "Gets a gameObject by name."; }
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
            Add("Name", PropertyType.String, null, null, PropertySource.EnteredOrInput);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var gameObjectName = (string) ValueOf("Name");
            var property = GameObject.Find(gameObjectName);
            if(property == null) throw new NodeParameterNotFoundException("GameObject is null.");
            var result = func(property);
            return result;
        }
    }
}