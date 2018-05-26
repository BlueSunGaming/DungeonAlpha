using System;
using System.Collections.Generic;
using Assets.Scripts.Exceptions;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Conversion")]
    public class GameObjectToPositionNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Vector3; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Object; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "GameObject To Vector3"; }
        }

        public override string Description
        {
            get { return "Get's the position of a gameobject"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var gameObject = (GameObject)ValueOf("GameObject");
            if(gameObject == null) throw new NodeParameterNotFoundException("GameObject is null");

            var position = gameObject.transform.position;
            var result = position;
            if(func != null)
            {
                var xx = (RPGVector3)func(position);
                result = (RPGVector3) xx;
            }
            return new RPGVector3(result);
        }

        protected override void SetupParameters()
        {
            Add("GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object);
        }
    }
}