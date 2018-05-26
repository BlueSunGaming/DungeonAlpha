using System;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Reputation Value")]
    public class GetReputationValue : PropertyNode
    {
        public override string Name
        {
            get { return "Get Reputation Value"; }
        }

        public override string Description
        {
            get { return "Gets the reputation value from the player"; }
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
            Add("Reputation", PropertyType.ReputationDefinition, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var player = GetObject.PlayerCharacter;
            var reputation = (string)ValueOf("Reputation");

            if (player != null)
            {
                var rep = GetObject.PlayerSave.QuestLog.AllReputations.FirstOrDefault(r => r.ReputationID == reputation);
                if (rep != null)
                {
                    if (func != null)
                    {
                        var result = Convert.ToInt32(func(rep.Value));
                        rep.Value = result;
                    }
                    return rep.Value;
                }
            }

            Debug.LogError("Reputation ID [" + reputation + "] not found");
            return 0;
        }
    }
}