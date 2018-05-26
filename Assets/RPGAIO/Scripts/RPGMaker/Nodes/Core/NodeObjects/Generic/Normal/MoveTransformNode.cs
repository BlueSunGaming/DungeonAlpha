using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class MoveTransformNode : SimpleNode
    {
        public override string Name
        {
            get { return "Move Transform"; }
        }

        public override string Description
        {
            get { return "Moves a transform."; }
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
            Add("Position", PropertyType.Vector3,null, RPGVector3.Zero, PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var position = (RPGVector3)ValueOf("Position");
            ApplyFunctionTo("GameObject", o =>
                                              {
                                                  var g = o as GameObject;
                                                  g.transform.position = position;
                                                  return o;
                                              });
        }

        
    }
}