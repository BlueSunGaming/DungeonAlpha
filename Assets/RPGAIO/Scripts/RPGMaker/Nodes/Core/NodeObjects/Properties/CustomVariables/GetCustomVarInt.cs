using System;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Combat Stats")]
    public class GetCustomVarInt : PropertyNode
    {
        public override string Name
        {
            get { return "Get Custom Variable Int"; }
        }

        public override string Description
        {
            get { return "Gets the custom variable's int value"; }
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
            Add("Custom Variable", PropertyType.Rmh_CustomVariable, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var id = (string)ValueOf("Custom Variable");
            var customVariable = GetObject.PlayerSave.GetCustomVariable(id); 
            
            if (func != null)
            {
                var result = Convert.ToInt32(func(customVariable.IntValue));
                customVariable.IntValue = result;
            }

            return customVariable.IntValue;
        }
    }
}