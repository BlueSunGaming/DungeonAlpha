using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Debug", "")]
    public class DebugIntValueNode : SimpleNode
    {
        public override string Name
        {
            get { return "Debug Int Value"; }
        }

        public override string Description
        {
            get { return "Logs a value to console."; }
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
            Add("Attr", PropertyType.Int, null, null, PropertySource.InputOnly, PropertyFamily.Primitive);
            Add("Identifier", PropertyType.String, null, "", PropertySource.EnteredOnly, PropertyFamily.Primitive);

        }

        protected override void Eval(NodeChain nodeChain)
        {
            ApplyFunctionTo("Attr", o =>
                                        {
                                            var val = Convert.ToInt32(o);
                                            Debug.Log("[" + (string)ValueOf("Identifier") + "]:" + val);
                                            return val;
                                        });
        }
    }
}