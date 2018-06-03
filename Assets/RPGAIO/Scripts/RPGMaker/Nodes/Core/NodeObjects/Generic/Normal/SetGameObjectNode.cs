using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class SetGameObjectNode : SimpleNode
    {
        public override string Name
        {
            get { return "Set GameObject"; }
        }

        public override string Description
        {
            get { return "Sets a gameobject to a value."; }
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
            Add("GameObject", PropertyType.GameObject, null, null, PropertySource.InputOnly, PropertyFamily.Object);
            Add("Value", PropertyType.GameObject, null, null, PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var value = (GameObject)ValueOf("Value");
            ApplyFunctionTo("GameObject", o =>
                                              {    
                                                  o = value;
                                                  return (GameObject)o;
                                              });
        }
    }
}