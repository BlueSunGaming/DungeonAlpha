using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("GameObject", "")]
    public class DestroyGameObject : SimpleNode
    {

        public override string Name
        {
            get { return "Destroy GameObject"; }
        }

        public override string Description
        {
            get { return "Destroys a gameobject"; }
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
            Add("GameObject", PropertyType.GameObject, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
            Add("Destroy After", PropertyType.Float, null, 0);
        }
        
        protected override void Eval(NodeChain nodeChain)
        {
            var gameObject = (GameObject)ValueOf("GameObject");
            var destroyAfter = (float)ValueOf("Destroy After");

            Object.Destroy(gameObject, destroyAfter);
        }
    }
}