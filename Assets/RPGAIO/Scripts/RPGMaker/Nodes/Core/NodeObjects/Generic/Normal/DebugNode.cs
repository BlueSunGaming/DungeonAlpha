using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Debug", "")]
    public class DebugNode : SimpleNode
    {
        public override string Name
        {
            get { return "Debug"; }
        }

        public override string Description
        {
            get { return "Logs string to console."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Type", PropertyType.StringArray, new[] { "Normal", "Warning", "Error" }, 0);
            Add("Message", PropertyType.String, null, "");
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var valueToAdd = new[] {"Normal","Warning", "Error"}[Convert.ToInt32(ValueOf("Type"))];
            var msg = (string) ValueOf("Message");
            if(valueToAdd == "Normal")
            {
                Debug.Log(msg);
            }
            else if (valueToAdd == "Warning")
            {   
                Debug.LogWarning(msg);
            }
            else if(valueToAdd == "Error")
            {
                Debug.LogError(msg);
            }
        }
    }
}