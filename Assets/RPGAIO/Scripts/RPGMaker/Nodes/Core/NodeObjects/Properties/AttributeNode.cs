using System;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Player Stats")]
    public class AttributeNode : PropertyNode
    {
        public override string Name
        {
            get { return "Player Attribute Total"; }
        }

        public override string Description
        {
            get { return "Gets the player's attribute."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override PropertyFamily PropertyFamily
        {
            get { return PropertyFamily.Primitive; }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return PropertyType.Int; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override void SetupParameters()
        {
            Add("Name", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var attrName = (string) ValueOf("Name");
            var attr = GetObject.PlayerCharacter.GetAttribute(attrName).BaseValue;
            if (func != null)
            {
                var result = Convert.ToInt32(func(attr));
                GetObject.PlayerCharacter.GetAttribute(attrName).BaseValue = result;
            }

            return GetObject.PlayerCharacter.GetAttribute(attrName).TotalValue;
        }
    }
}